using System;
using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Rules.ValidationRules.UserAccount;

namespace Transversals.Business.UserPermissions.Domain.UserAccountValidationRules
{
    /// <inheritdoc/>
    public class CheckDate : ValidationRule<UserAccount>, ICheckDate
    {
        /// <inheritdoc/>
        public override IValidationRuleResult Execute()
        {
            if (Item.FromDate > Item.ToDate)
            {
                return Error(Transversals.Business.Domain.Properties.Resources.UserPermissions.SavingDateErrorMessage);
            }

            return Success();
        }
    }
}