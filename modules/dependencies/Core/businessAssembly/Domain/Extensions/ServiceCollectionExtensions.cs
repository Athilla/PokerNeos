using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using Transversals.Business.Core.Domain.Configuration;
using Transversals.Business.Core.Domain.Configuration.Counters;
using Transversals.Business.Core.Domain.Configuration.Options;

namespace Transversals.Business.Core.Domain.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreConfiguration(this IServiceCollection services)
        {
            services.TryAddScoped<ICounterService, CounterService>();
            services.TryAddScoped<IOptionService, OptionService>();
            services.TryAddScoped<ICoreConfiguration, CoreConfiguration>();
            return services;
        }

        public static IServiceCollection AddCoreConfiguration(this IServiceCollection services, Action<ICoreConfigurationBuilder> configure)
        {
            services.AddCoreConfiguration();

            configure(new CoreConfigurationBuilder(services));
            return services;
        }

    }
}
