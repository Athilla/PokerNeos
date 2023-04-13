using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Transversals.Business.Authentication.Application.AuthenticationNeos;
using Transversals.Business.Authentication.Application.AzureADB2C;
using Transversals.Business.Authentication.Application.Factory;
using Transversals.Business.Authentication.Application.Services;
using Transversals.Business.Authentication.Domain.AzureADB2C;

namespace Transversals.Business.Authentication.Application
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

        }
    }
}