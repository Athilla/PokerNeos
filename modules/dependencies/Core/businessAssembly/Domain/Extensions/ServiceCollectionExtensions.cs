using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using Transversals.Business.Core.Domain.Configuration;
using Transversals.Business.Core.Domain.Configuration.Counters;
using Transversals.Business.Core.Domain.Configuration.Options;
using Transversals.Business.Core.Domain.MemoryCache;

namespace Transversals.Business.Core.Domain.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreConfiguration(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddScoped<ICounterService, CounterService>();
            services.TryAddScoped<IOptionService, OptionService>();
            services.TryAddScoped<ICoreConfiguration, CoreConfiguration>();
            services.TryAddScoped<ICoreMemoryCache, CoreMemoryCache>();
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
