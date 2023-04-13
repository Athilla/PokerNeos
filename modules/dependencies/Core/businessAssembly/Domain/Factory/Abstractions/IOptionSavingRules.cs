using FluentResults;

namespace Transversals.Business.Core.Domain.Factory.Abstractions
{
    /// <summary>
    /// Interface for saving option events
    /// </summary>
    public interface IOptionSavingRules
    {

        /// <summary>
        /// Savings execution.
        /// </summary>
        /// <returns></returns>
        Result Saving(Transversals.Business.Domain.Entities.Option opt);
    }
}
