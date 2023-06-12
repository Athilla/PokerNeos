using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using GroupeIsa.Neos.Shared.Linq;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Enums;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.UserPermissions.Domain.MemoryCache;

namespace Transversals.Business.UserPermissions.Domain.PermissionEventRules
{
    /// <summary>
    /// Represents Saved event rule.
    /// </summary>
    public class Saved : ISavedRule<Permission>
    {
        private readonly IUserAccountUserRoleRepository _userAccountUserRoleRepository;
        private readonly ICoreMemoryCache _coreMemoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saved"/> class.
        /// </summary>
        public Saved(IUserAccountUserRoleRepository userAccountUserRoleRepository,
            ICoreMemoryCache coreMemoryCache)
        {
            _userAccountUserRoleRepository = userAccountUserRoleRepository;
            _coreMemoryCache = coreMemoryCache;
        }

        /// <inheritdoc/>
        public async Task OnSavedAsync(ISavedRuleArguments<Permission> args)
        {
            foreach (Permission item in args.CreatedAndModifiedItems)
            {
                RemoveCache(_userAccountUserRoleRepository.GetQuery()
                .Include(e => e.UserAccount)
                .Where(e => e.UserRoleId == item.UserRoleId)
                .Select(e => e.UserAccount.Email));
            }

            foreach (Permission deletedItem in args.DeletedItems)
            {
                RemoveCache(_userAccountUserRoleRepository.GetQuery()
                .Include(e => e.UserAccount)
                .Where(e => e.UserRoleId == deletedItem.UserRoleId)
                .Select(e => e.UserAccount.Email));
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Remove memory cache for given key
        /// </summary>
        /// <param name="userMail"></param>
        private void RemoveCache(IEnumerable<string> userEmails)
        {
            foreach (string email in userEmails)
            {
                _coreMemoryCache.Remove(MemoryCacheCategories.Permissions.ToString(), email);
            }
        }
    }
}