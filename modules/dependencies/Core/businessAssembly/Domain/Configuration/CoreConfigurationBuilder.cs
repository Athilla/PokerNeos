using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Transversals.Business.Core.Domain.Configuration
{
    /// <summary>
    ///  An interface for configuring CoreConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class CoreConfigurationBuilder : ICoreConfigurationBuilder
    {

        public CoreConfigurationBuilder(IServiceCollection services)
        {
            Services = services;
        }
        public IServiceCollection Services { get; }
    }
}
