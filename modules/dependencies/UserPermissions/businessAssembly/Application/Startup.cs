using System.Diagnostics.CodeAnalysis;
using Flurl.Http.Configuration;
using GroupeIsa.Neos.Application.Extensions;
using GroupeIsa.Neos.Application.Permissions;
using GroupeIsa.Neos.Migration.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;
using Transversals.Business.UserPermissions.Application.AuthenticationNeos;
using Transversals.Business.UserPermissions.Application.AzureADB2C;
using Transversals.Business.UserPermissions.Application.Factory;
using Transversals.Business.UserPermissions.Application.Migration;
using Transversals.Business.UserPermissions.Application.PermissionNeos;
using Transversals.Business.UserPermissions.Application.Services;

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
            services.AddMemoryCache();
            services.AddScoped<IPermissionLoader, PermissionLoader>();
            services.AddSingleton<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>();
            services.AddScoped<AuthenticationNeosClient>();
            services.AddScoped<AzureB2CClient>();
            services.AddScoped<IFactoryAuthProvider, FactoryAuthProvider>();
            services.AddScoped<IGraphService, GraphService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSendGrid((sp, option) =>
            {
                var configuration = sp.GetService<IConfiguration>();
                if (configuration != null)
                {
                    option.ApiKey = configuration["SendGrid:Key"];
                }
            });
            services.AddTenantDatabaseMigrationInterceptor<UserPermissionsTenantDatabaseMigrationInterceptor>();
            services.AddMigrationInterceptor<UserPermissionsMigrationInterceptor>();
            services.AddTenantResolvedInterceptor<TenantResolvedInterceptor>();
        }
    }
}