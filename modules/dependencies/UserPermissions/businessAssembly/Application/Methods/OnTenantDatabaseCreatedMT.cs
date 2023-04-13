using FluentResults;
using GroupeIsa.Neos.Application.Permissions;
using GroupeIsa.Neos.Domain.Persistence;
using GroupeIsa.Neos.Shared.Logging;
using System;
using System.Linq;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Core.Domain.HostedServices;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.UserPermissions.Application.Methods
{
    /// <summary>
    /// Represents OnTenantDatabaseCreated method.
    /// </summary>
    public class OnTenantDatabaseCreatedMT : IOnTenantDatabaseCreatedMT
    {
        private readonly INeosLogger<IOnTenantDatabaseCreatedMT> _logger;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFunctionLoader _functionLoader;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaveUsersToAuthenticationServer _saveUsersToAuthenticationServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnTenantDatabaseCreated" /> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="saveUsersToAuthenticationServer">The save users to authentication server.</param>
        /// <param name="userAccountRepository">The user account repository.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public OnTenantDatabaseCreatedMT(INeosLogger<IOnTenantDatabaseCreatedMT> logger,
            ISaveUsersToAuthenticationServer saveUsersToAuthenticationServer,
            IUserAccountRepository userAccountRepository,
            IServiceProvider serviceProvider,
            IFunctionLoader functionLoader,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userAccountRepository = userAccountRepository;
            _serviceProvider = serviceProvider;
            _functionLoader = functionLoader;
            _unitOfWork = unitOfWork;
            _saveUsersToAuthenticationServer = saveUsersToAuthenticationServer;
        }


        /// <inheritdoc/>
        public async System.Threading.Tasks.Task<FluentResults.Result> ExecuteAsync(GroupeIsa.Neos.Application.MultiTenant.CreatedTenantInfo createdTenantInfo)
        {
            // Get or Create user in MT UserAccount repository
            UserAccount? existingAdminAccount = _userAccountRepository.GetQuery().FirstOrDefault(u => u.Login == createdTenantInfo.AdministratorLogin);

            UserAccount adminAccount;
            if (existingAdminAccount == null)
            {
                adminAccount = new()
                {
                    LastName = "Admin",
                    FirstName = "Admin",
                    Email = createdTenantInfo.AdministratorLogin,
                    Login = createdTenantInfo.AdministratorLogin,
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
                    adminRole.Permissions.Add(new Domain.Entities.Permission
                    {
                        FunctionName = function.Name,
                        FunctionType = Transversals.Business.Domain.Enums.FunctionType.AllowDeny,
                        HasAccess = true
                    });
                }
                else
                {
                    adminRole.Permissions.Add(new Domain.Entities.Permission
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
                Email = adminAccount.Email,
                FirstName = adminAccount.FirstName,
                LastName = adminAccount.LastName,
            };

            Result<Result> exceptionalResult = await Result.Try(_unitOfWork.SaveAsync);
            if (exceptionalResult.IsFailed)
            {
                return exceptionalResult.ToResult();
            }

            Result<ServerMethodResult> creationUserResult = await Result.Try(() => _saveUsersToAuthenticationServer.ExecuteAsync(new[] { adminAuthentication }));
            if (creationUserResult.IsFailed)
            {
                return creationUserResult.ToResult();
            }

            if (creationUserResult.Value.Message != null)
            {
                return Result.Fail($"Failed to create user : {creationUserResult.Value.Message}");
            }

            //Gestion des options et compteurs predefinit
            int nb = BackgroundTaskList.Instance.Count;
            for (int i = 0; i < nb; i++)
            {
                var data = BackgroundTaskList.Instance.Get(i);
                if (data != null)
                {
                    try
                    {
                        _logger.LogDebug("Treating data...");
                        var task = data.Invoke(_serviceProvider);
                        await task.WaitAsync(new TimeSpan(0, 0, 1));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error when invoking lambda");
                        return Result.Fail($"Failed to initialize tenant");
                    }
                }
            }
            return Result.Ok();
        }
    }
}