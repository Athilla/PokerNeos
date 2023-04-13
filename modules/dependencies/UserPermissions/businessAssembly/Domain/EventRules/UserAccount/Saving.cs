using GroupeIsa.Neos.Application.MultiTenant;
using GroupeIsa.Neos.ClusterCommunication;
using GroupeIsa.Neos.Domain.Exceptions;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using GroupeIsa.Neos.Shared.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.UserPermissions.Domain.UserAccountEventRules
{
    /// <summary>
    /// Represents Saving event rule.
    /// </summary>
    public class Saving : ISavingRule<UserAccount>
    {
        private readonly INeosLogger<ISavingRule<UserAccount>> _logger;
        private readonly ITenants _tenants;
        private readonly IRemoteClusterServiceInvoker _remoteClusterServiceInvoker;
        private readonly string _regexValidation = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        private readonly IUserAccountRepository _userAccountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saving" /> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="tenants">The tenants.</param>
        /// <param name="remoteClusterServiceInvoker">The remote cluster service invoker.</param>
        public Saving(INeosLogger<ISavingRule<UserAccount>> logger, 
            ITenants tenants, 
            IRemoteClusterServiceInvoker remoteClusterServiceInvoker,
            IUserAccountRepository userAccountRepository)
        {
            _logger = logger;
            _tenants = tenants;
            _remoteClusterServiceInvoker = remoteClusterServiceInvoker;
            _userAccountRepository = userAccountRepository;
        }

        /// <inheritdoc/>
        public async Task OnSavingAsync(ISavingRuleArguments<UserAccount> args)
        {
            await Task.CompletedTask;

            foreach(var item in args.ModifiedItems)
            {
                UserAccount originalUser = _userAccountRepository.GetOriginal(item);
                if (originalUser.Login != item.Login || originalUser.Email != item.Email)
                {
                    throw new BusinessException(Resources.UserPermissions.UserAccountMailAndLoginAreReadOnlyMessage);
                }
            }
            foreach (var item in args.CreatedAndModifiedItems)
            {
                if (item.Email != item.Email.ToLower())
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