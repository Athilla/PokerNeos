using System;
using System.Threading.Tasks;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using Transversals.Business.Domain.Entities;

namespace Transversals.Business.UserPermissions.Domain.UserRoleEventRules
{
    /// <summary>
    /// Represents Saved event rule.
    /// </summary>
    public class Saved : ISavedRule<UserRole>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saved"/> class.
        /// </summary>
        public Saved()
        {
        }

        /// <inheritdoc/>
        public Task OnSavedAsync(ISavedRuleArguments<UserRole> args)
        {
            // L'action 'invalidateCachePermissionsAction' est définie dans la règle saving.
            if (args.Context.TryGet(Saving.ContextVariable_InvalidateCachePermissions, out Action? invalidateCachePermissionsAction))
            {
                invalidateCachePermissionsAction();
            }

            return Task.CompletedTask;
        }
    }
}