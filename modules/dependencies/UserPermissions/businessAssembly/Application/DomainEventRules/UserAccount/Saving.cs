using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GroupeIsa.Neos.Application.MultiTenant;
using GroupeIsa.Neos.ClusterCommunication;
using GroupeIsa.Neos.Domain.Exceptions;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using GroupeIsa.Neos.Shared.Logging;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.Domain.Properties;
using Transversals.Business.UserPermissions.Application.AuthenticationNeos;
using Transversals.Business.UserPermissions.Application.Factory;
using static Transversals.Business.UserPermissions.Application.AuthenticationNeos.UserAuthenticationResult;

namespace Transversals.Business.UserPermissions.Application.UserAccountDomainEventRules
{
    /// <summary>
    /// Represents Saving event rule.
    /// </summary>
    public class Saving : ISavingRule<UserAccount>
    {
        private readonly ISavingRule<UserAccount> _baseRule;
        private readonly INeosLogger<ISavingRule<UserAccount>> _logger;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ITenants _tenants;
        private readonly IRemoteClusterServiceInvoker _remoteClusterServiceInvoker;
        private readonly IEnumerable<IFactoryAuthProvider> _factoryAuthProvider;
        private readonly string _regexValidation = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        /// <summary>
        /// Initializes a new instance of the <see cref="Saving"/> class.
        /// </summary>
        /// <param name="baseRule">BaseRule.</param>
        /// <param name="logger">Logger.</param>
        public Saving(ISavingRule<UserAccount> baseRule,
            INeosLogger<ISavingRule<UserAccount>> logger,
            IUserAccountRepository userAccountRepository,
            ITenants tenants,
            IRemoteClusterServiceInvoker remoteClusterServiceInvoker,
            IEnumerable<IFactoryAuthProvider> factoryAuthProvider)
        {
            _baseRule = baseRule;
            _logger = logger;
            _userAccountRepository = userAccountRepository;
            _tenants = tenants;
            _remoteClusterServiceInvoker = remoteClusterServiceInvoker;
            _factoryAuthProvider = factoryAuthProvider;
        }

        /// <inheritdoc/>
        public async Task OnSavingAsync(ISavingRuleArguments<UserAccount> args)
        {
            await _baseRule.OnSavingAsync(args);
            foreach (var item in args.ModifiedItems)
            {
                UserAccount originalUser = _userAccountRepository.GetOriginal(item);
                if (originalUser.Login != item.Login || originalUser.Email != item.Email)
                {
                    throw new BusinessException(Resources.UserPermissions.UserAccountMailAndLoginAreReadOnlyMessage);
                }
            }
            foreach (var item in args.CreatedAndModifiedItems)
            {
                item.Email = item.Email.ToLower();

                if (!Regex.IsMatch(item.Email, _regexValidation))
                {
                    args.Cancel = true;
                    _logger.LogError($"Email is invalid {item.Email}");
                }
            }
            if (_tenants.MultitenancyEnabled)
            {
                foreach (var newAccount in args.CreatedItems)
                {
                    await AssignUserToTenantAsync(newAccount.Login, newAccount.FirstName, newAccount.LastName);
                }
                foreach (var removeAccount in args.DeletedItems)
                {
                    await UnassignUserFromTenantAsync(removeAccount.Login);
                }
            }

            // Only unsynchronized users need to be registered with the authentication provider.
            UserAccount[] unsynchronizedUsers = args.CreatedItems.Where(u => !u.Synchronized).ToArray();

            UserAuthentication[] userList = unsynchronizedUsers
                .Select(item => new UserAuthentication()
                {
                    Name = item.Login,
                    Email = item.Email,
                    Language = "French",
                    FirstName = item.FirstName,
                    LastName = item.LastName
                }).ToArray();

            await PostUserToAuthenticationServerAsync(unsynchronizedUsers, userList);
        }

        /// <summary>
        /// Posts the user to authentication server.
        /// </summary>
        /// <param name="createdItems">The created items.</param>
        /// <param name="userList">The user list.</param>
        private async Task PostUserToAuthenticationServerAsync(UserAccount[] createdItems, UserAuthentication[] userList)
        {
            if (userList.Any())
            {
                List<UserAuthenticationResult> result = new List<UserAuthenticationResult>();
                foreach (var factoryAuthProvider in _factoryAuthProvider)
                {
                    var provider = factoryAuthProvider.GetProvider();
                    if (provider != null)
                        result.AddRange(await provider.RegisterUsersAsync(userList));
                }

                if (!result.Any())
                {
                    throw new BusinessException(Resources.UserPermissions.SaveUsersToAuthenticationServerFailed);
                }

                //récupération des utilisateurs en échec
                List<UserAuthenticationResult> failedsUser = new();
                if (!result.Select(r => r.Name).SequenceEqual(userList.Select(u => u.Name)))
                {
                    IEnumerable<UserAuthenticationResult> namesNotInResult = userList.Where(u => !result.Select(r => r.Name).Contains(u.Name))
                                                                            .Select(u => new UserAuthenticationResult() { Name = u.Name, State = StateType.Failed });
                    failedsUser.AddRange(namesNotInResult);
                }

                if (result.Any(r => r.State != UserAuthenticationResult.StateType.Succeeded
                                 && r.State != UserAuthenticationResult.StateType.AlreadyCreated))
                {
                    failedsUser.AddRange(result
                               .Where(r => r.State != UserAuthenticationResult.StateType.Succeeded
                                        && r.State != UserAuthenticationResult.StateType.AlreadyCreated));
                }

                if (failedsUser.Any())
                {
                    throw new BusinessException(GetErrorMessage(failedsUser));
                }

                foreach (UserAuthenticationResult userAuthenticationResult in result)
                {
                    if (userAuthenticationResult.State == UserAuthenticationResult.StateType.Succeeded
                        || userAuthenticationResult.State == UserAuthenticationResult.StateType.AlreadyCreated)
                    {
                        createdItems.Where(c => c.Login == userAuthenticationResult.Name)
                                    .ToList()
                                    .ForEach(c => c.Synchronized = true);
                    }
                }
            }
        }

        /// <summary>
        /// Retourne un texte d'erreur pour la liste d'utilisateurs non synchronisés
        /// </summary>
        /// <param name="failedsUserNames">Login des utilisateurs en erreur</param>
        /// <returns>Message d'erreur ou <c>null</c></returns>
        private static string GetErrorMessage(List<UserAuthenticationResult> failedsUser)
        {
            string message = string.Join($"{Environment.NewLine}- ", failedsUser.Distinct().Select(e => ParseError(e)));
            return string.Format(Resources.UserPermissions.SaveUsersToAuthenticationServerFailedForUsers, message);
        }

        private static string? ParseError(UserAuthenticationResult userAuthResult)
        {
            switch (userAuthResult.State)
            {
                case UserAuthenticationResult.StateType.AlreadyCreatedAndDifferentEmail:
                    return $"{userAuthResult.Name} {Resources.UserPermissions.AuthNeosErrorLoginAlreadyUsedEmail}";
                default:
                    return userAuthResult.Name;
            }
        }

        private Task AssignUserToTenantAsync(string userIdentifier, string firstName, string lastName)
        {
            return _remoteClusterServiceInvoker.InvokeMethodPostAsync(
                 "tenants",
                 "v1",
                 "AssignUserToTenant",
                 new Dictionary<string, object?>
                 {
                     ["tenantIdentifier"] = _tenants.GetCurrentTenantIdentifier(),
                     ["userIdentifier"] = userIdentifier,
                     ["userFirstName"] = firstName,
                     ["userLastName"] = lastName,
                 });

        }
        private Task UnassignUserFromTenantAsync(string userIdentifier)
        {
            return _remoteClusterServiceInvoker.InvokeMethodPostAsync(
                 "tenants",
                 "v1",
                 "UnassignUserToTenant",
                 new Dictionary<string, object?>
                 {
                     ["tenantIdentifier"] = _tenants.GetCurrentTenantIdentifier(),
                     ["userIdentifier"] = userIdentifier
                 });
        }
    }
}