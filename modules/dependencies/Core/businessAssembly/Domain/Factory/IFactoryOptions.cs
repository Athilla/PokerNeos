using System;
using Transversals.Business.Core.Domain.Factory.Abstractions;
using Transversals.Business.Domain.Entities;

namespace Transversals.Business.Core.Domain.Factory
{
    public interface IFactoryOptions
    {
        /// <summary>
        /// Gets the option saving rules.
        /// </summary>
        /// <param name="opt">The opt.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        IOptionSavingRules? GetOptionSavingRules(Option opt, IServiceProvider serviceProvider);
    }
}