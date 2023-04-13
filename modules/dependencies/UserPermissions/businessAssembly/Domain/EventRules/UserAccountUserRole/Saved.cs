using GroupeIsa.Neos.Domain.Rules.EventRules;
using System.Threading.Tasks;
using Transversals.Business.Core.Domain.MemoryCache;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Enums;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.UserPermissions.Domain.UserAccountUserRoleEventRules
{
    /// <summary>
    /// Represents Saved event rule.
    /// </summary>
    public class Saved : ISavedRule<UserAccountUserRole>
    {
        private readonly ICoreMemoryCache _coreMemoryCache;
        private readonly IUserAccountRepository _userAccountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saved"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public Saved(ICoreMemoryCache coreMemoryCache,
            IUserAccountRepository userAccountRepository)
        {
            _coreMemoryCache = coreMemoryCache;
            _userAccountRepository = userAccountRepository;
        }

        /// <inheritdoc/>
        public async Task OnSavedAsync(ISavedRuleArguments<UserAccountUserRole> args)
        {
            foreach (UserAccountUserRole item in args.CreatedAndModifiedItems)
            {
                UserAccount? userAccount = await _userAccountRepository.FindAsync(item.UserAccountId);
                if(userAccount != null)
                {
                    RemoveCache(userAccount.Email);
                }
            }

            foreach (UserAccountUserRole deletedItem in args.DeletedItems)
            {
                UserAccount? userAccount = await _userAccountRepository.FindAsync(deletedItem.UserAccountId);
                if (userAccount != null)
                {
                    RemoveCache(userAccount.Email);
                }
            }
        }

        /// <summary>
        /// Remove memory cache for given key
        /// </summary>
        /// <param name="userMail"></param>
        private void RemoveCache(string email)
        {
            _coreMemoryCache.Remove(MemoryCacheCategories.Permissions.ToString(), email);
        }
    }
}