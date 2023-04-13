using GroupeIsa.Neos.Application.Permissions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Transversals.Business.UserPermissions.Application.PermissionNeos;

namespace Transversals.Business.UserPermissions.Application
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
            services.AddScoped<IPermissionLoader, PermissionLoader>();
        }
    }
}