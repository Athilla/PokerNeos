using System;
using System.Linq;
using System.Threading.Tasks;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using GroupeIsa.Neos.Shared.Linq;
using GroupeIsa.Neos.Shared.Logging;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Enums;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.UserPermissions.Domain.MemoryCache;

namespace Transversals.Business.UserPermissions.Domain.UserRoleEventRules
{
    /// <summary>
    /// Represents Saving event rule.
    /// </summary>
    public class Saving : ISavingRule<UserRole>
    {
        private readonly INeosLogger<ISavingRule<UserRole>> _logger;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICoreMemoryCache _coreMemoryCache;

        public const string ContextVariable_InvalidateCachePermissions = "InvalidateCachePermissions";

        /// <summary>
        /// Initializes a new instance of the <see cref="Saving"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="coreMemoryCache">Core memory cache.</param>
        /// <param name="userRoleRepository">User role repository.</param>
        public Saving(INeosLogger<ISavingRule<UserRole>> logger, IUserRoleRepository userRoleRepository, ICoreMemoryCache coreMemoryCache)
        {
            _logger = logger;
            _userRoleRepository = userRoleRepository;
            _coreMemoryCache = coreMemoryCache;
        }


        /// <inheritdoc/>
        public Task OnSavingAsync(ISavingRuleArguments<UserRole> args)
        {
            var roleIdList = args.ModifiedItems.Concat(args.DeletedItems).Select(r => r.Id);

            // Récupération des informations complète des rôles modifiés/créés en incluant les informations nécessaire au traitement de saved
            // Cet étape est nécessaire pour 2 raisons
            UserRole[] roles = _userRoleRepository
                .GetQuery()
                .Where(r => roleIdList.Contains(r.Id))
                .Include(u => u.Accounts)
                .ThenInclude(a => a.UserAccount)
                .ToArray();

            // Le traitement à lancer est passer en contexte car il sera exécuté au saved lorsque les données seront validées/persistées avec succès.
            args.Context.TryAdd(ContextVariable_InvalidateCachePermissions, () => InvalidateCachePermissions(roles));

            return Task.CompletedTask;
        }

        internal void InvalidateCachePermissions(UserRole[] roles)
        {
            string[] emails = roles.SelectMany(r => r.Accounts.Select(a => a.UserAccount.Email)).ToArray();

            foreach (string email in emails)
            {
                _coreMemoryCache.Remove(MemoryCacheCategories.Permissions.ToString(), email);
            }

            _logger.LogInformation($"Cache permission invalidated for emails : {string.Join(',', emails)}");
        }
    }
}