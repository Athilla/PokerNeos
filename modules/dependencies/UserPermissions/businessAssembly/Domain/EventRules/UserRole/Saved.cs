using GroupeIsa.Neos.Domain.Rules.EventRules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Core.Domain.MemoryCache;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Enums;

namespace Transversals.Business.UserPermissions.Domain.UserRoleEventRules
{
    public class Saved : ISavedRule<UserRole>
    {
        private readonly ICoreMemoryCache _coreMemoryCache;

        public Saved(ICoreMemoryCache coreMemoryCache)
        {
            _coreMemoryCache = coreMemoryCache;
        }

        public async Task OnSavedAsync(ISavedRuleArguments<UserRole> args)
        {
            foreach (var item in args.CreatedAndModifiedItems)
            {
                RemoveCache(item.Accounts.Select(a => a.UserAccount.Email));
            }

            foreach (var deletedItem in args.DeletedItems)
            {
                RemoveCache(deletedItem.Accounts.Select(a => a.UserAccount.Email));
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
