using GroupeIsa.Neos.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Transversals.Business.Core.Domain.Extensions;
using Transversals.Business.Core.Domain.Factory;
using Transversals.Business.Core.Domain.Migration;

namespace Transversals.Business.Core.Domain
{
    /// <summary>
    /// Represents the assembly startup.
    /// </summary>
    /// <remarks>
    /// This class is automatically instanciated when the assembly is loaded.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public static class Startup
    {
        /// <summary>
        /// Configures services for dependency injection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <remarks>
        /// This method is automatically called when the assembly is loaded.
        /// </remarks>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCoreConfiguration();
            services.AddScoped<IFactoryOptions, FactoryOptions>();
            services.AddTenantDatabaseMigrationInterceptor<MTTenantDatabaseMigrationInterceptor>();
        }

    }
}