using System.Linq;
using System.Threading.Tasks;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using Transversals.Business.Domain.Entities;

namespace Transversals.Business.Core.Domain.CounterEventRules
{
    /// <summary>
    /// Represents Saving event rule.
    /// </summary>
    public class Saving : ISavingRule<Counter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saving"/> class.
        /// </summary>
        public Saving()
        {

        }

        /// <inheritdoc/>
        public async Task OnSavingAsync(ISavingRuleArguments<Counter> args)
        {
            if(args.DeletedItems.Any(item => item.Locked))
            {
                args.Cancel = true;
            }
            await Task.CompletedTask;
        }
    }
}