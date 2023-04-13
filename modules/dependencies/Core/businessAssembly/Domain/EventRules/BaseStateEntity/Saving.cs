using GroupeIsa.Neos.Domain.Rules.EventRules;
using System;
using System.Threading.Tasks;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.Core.Domain.BaseStateEntityEventRules
{

    /// <summary>
    /// Represents Saving event rule.
    /// </summary>
    public class Saving : ISavingRule<Transversals.Business.Domain.Entities.BaseStateEntity>
    {
        /// <inheritdoc/>
        public Task OnSavingAsync(ISavingRuleArguments<Transversals.Business.Domain.Entities.BaseStateEntity> args)
        {
            foreach (Transversals.Business.Domain.Entities.BaseStateEntity item in args.CreatedAndModifiedItems)
            {
                if (DateTime.Compare(item.FromDate ?? DateTime.Today, item.ToDate ?? DateTime.Today) > 0)
                {
                    throw new GroupeIsa.Neos.Domain.Exceptions.BusinessException(Resources.Core.SavingDateErrorMessage);
                }
            }

            return Task.CompletedTask;
        }
    }
}