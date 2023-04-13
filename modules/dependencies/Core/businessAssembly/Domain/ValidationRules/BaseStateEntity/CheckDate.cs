using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using System;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Properties;
using Transversals.Business.Domain.Rules.ValidationRules.BaseStateEntity;

namespace Transversals.Business.Core.Domain.BaseStateEntityValidationRules
{
    /// <inheritdoc/>
    public class CheckDate : ValidationRule<BaseStateEntity>, ICheckDate
    {
        /// <inheritdoc/>
        public override IValidationRuleResult Execute()
        {
            if (Item.FromDate != null
                && Item.ToDate != null
                && DateTime.Compare((DateTime)Item.FromDate, (DateTime)Item.ToDate) > 0)
            {
                return Error(Resources.Core.SavingDateErrorMessage);
            }

            return Success();
        }
    }
}