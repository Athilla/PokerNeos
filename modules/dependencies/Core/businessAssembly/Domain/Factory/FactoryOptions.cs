using System;
using Transversals.Business.Core.Domain.Factory.Abstractions;
using Transversals.Business.Domain.Entities;

namespace Transversals.Business.Core.Domain.Factory
{
    /// <summary>
    /// FactoryOptions used to assignate <see cref="IOptionSavingRules"/> to saving option event
    /// </summary>
    public class FactoryOptions : IFactoryOptions
    {
        /// <summary>
        /// Gets the option saving rules.
        /// </summary>
        /// <param name="optionName">Name of the option.</param>
        /// <param name="opt">The opt.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        public IOptionSavingRules? GetOptionSavingRules(Option opt, IServiceProvider serviceProvider)
        {
            return opt.Name switch
            {
                _ => null
            };
        }
    }
}
