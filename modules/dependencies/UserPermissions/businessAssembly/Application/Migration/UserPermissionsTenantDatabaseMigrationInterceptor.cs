using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using GroupeIsa.Neos.Application.MultiTenant;
using GroupeIsa.Neos.Application.Permissions;
using GroupeIsa.Neos.Domain.Persistence;
using GroupeIsa.Neos.Shared.MultiTenant;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.UserPermissions.Application.Migration
{
    /// <summary>
    /// Represents <see cref="ITenantDatabaseMigrationInterceptor"/> implementation to prepare and migrate data of user permissions.
    /// </summary>
    internal class UserPermissionsTenantDatabaseMigrationInterceptor : ITenantDatabaseMigrationInterceptor
    {
        private readonly ISaveUsersToAuthenticationServer _saveUsersToAuthenticationServer;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IFunctionLoader _functionLoader;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantsDataMigrator"/> class.
        /// </summary>
        /// <param name="saveUsersToAuthenticationServer">The save users to authentication server.</param>
        /// <param name="userAccountRepository">The user account repository.</param>
        /// <param name="functionLoader">The function loader.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public UserPermissionsTenantDatabaseMigrationInterceptor(ISaveUsersToAuthenticationServer saveUsersToAuthenticationServer,
            IUserAccountRepository userAccountRepository,
            IFunctionLoader functionLoader,
            IUnitOfWork unitOfWork)
        {
            _saveUsersToAuthenticationServer = saveUsersToAuthenticationServer;
            _userAccountRepository = userAccountRepository;
            _functionLoader = functionLoader;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task<Result> OnTenantDatabaseMigratedAsync(TenantDatabaseMigratedEventData tenantDatabaseMigratedEventData)
        {
            if (tenantDatabaseMigratedEventData.AdministratorLogin == null)
            {
                return Result.Ok();
            }

            // Get or Create user in MT UserAccount repository
            UserAccount? existingAdminAccount = _userAccountRepository.GetQuery().FirstOrDefault(u => u.Login == tenantDatabaseMigratedEventData.AdministratorLogin);

            UserAccount adminAccount;
            if (existingAdminAccount == null)
            {
                adminAccount = new()
                {
                    LastName = "Admin",
                    FirstName = "Admin",
                    Email = tenantDatabaseMigratedEventData.AdministratorLogin,
                    Login = tenantDatabaseMigratedEventData.AdministratorLogin,
                    IsActive = true,
                };

                _userAccountRepository.Add(adminAccount);

            }
            else
            {
                adminAccount = existingAdminAccount;
            }

            // Check or add administrator Role
            UserRole adminRole = new UserRole()
            {
                Name = "Administrator",
            };
            foreach (var function in _functionLoader.GetFunctions())
            {
                if (function.PermissionType == GroupeIsa.Neos.Application.Permissions.PermissionType.AllowDeny)
                {
                    adminRole.Permissions.Add(new Transversals.Business.Domain.Entities.Permission
                    {
                        FunctionName = function.Name,
                        FunctionType = Transversals.Business.Domain.Enums.FunctionType.AllowDeny,
                        HasAccess = true
                    });
                }
                else
                {
                    adminRole.Permissions.Add(new Transversals.Business.Domain.Entities.Permission
                    {
                        FunctionName = function.Name,
                        FunctionType = Transversals.Business.Domain.Enums.FunctionType.CRUD,
                        HasCreationAccess = true,
                        HasDeleteAccess = true,
                        HasUpdateAccess = true,
                        HasReadOnlyAccess = false
                    });
                }
            }

            adminAccount.Roles.Add(new UserAccountUserRole()
            {
                UserAccount = adminAccount,
                UserRole = adminRole
            });
            UserAuthentication adminAuthentication = new()
            {
                Name = adminAccount.Login,
                Email = adminAccount.Email,
                FirstName = adminAccount.FirstName,
                LastName = adminAccount.LastName,
            };

            Result<Result> exceptionalResult = await Result.Try(_unitOfWork.SaveAsync);
            if (exceptionalResult.IsFailed)
            {
                return exceptionalResult.ToResult();
            }

            Result<UserPermissionsMethodResult> creationUserResult = await Result.Try(() => _saveUsersToAuthenticationServer.ExecuteAsync(new[] { adminAuthentication }));
            if (creationUserResult.IsFailed)
            {
                return creationUserResult.ToResult();
            }

            if (creationUserResult.Value.Message != null)
            {
                return Result.Fail($"Failed to create user : {creationUserResult.Value.Message}");
            }

            return Result.Ok();
        }
    }
}
