using Microsoft.Extensions.DependencyInjection;

namespace Transversals.Business.Core.Domain.Configuration
{
    /// <summary>
    ///  An interface for configuring CoreConfiguration
    /// </summary>
    public interface ICoreConfigurationBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> where Core Configuration services are configured.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
