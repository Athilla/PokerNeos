using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Transversals.Business.Core.Domain.Configuration;
using Transversals.Business.Core.Domain.Configuration.Counters;
using Transversals.Business.Core.Domain.Configuration.Options;
using Transversals.Business.Core.Domain.HostedServices;

namespace Transversals.Business.Core.Domain.Extensions
{
    public static class CoreConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds the option.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="optionName">Name of the option.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static ICoreConfigurationBuilder AddOption<T>(this ICoreConfigurationBuilder builder, string optionName, T value)
        {
            Func<IServiceProvider, Task> function = async (sp) =>
            {
                var optionService = sp.GetRequiredService<IOptionService>();
                if (!optionService.TryFindOption(optionName))
                {
                    await optionService.CreateNewOptionAsync(optionName, value);
                }
            };

            BackgroundTaskQueue.Instance.QueueBackgroundWorkItem(function);
            BackgroundTaskList.Instance.Add(function);

            return builder;
        }

        /// <summary>
        /// Adds a counter.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <returns></returns>
        public static ICoreConfigurationBuilder AddCounter(this ICoreConfigurationBuilder builder, string counterName, string? prefix, string? suffix, int initialValue, int maxValue = int.MaxValue - 1)
        {
            Func<IServiceProvider, Task> function = async (sp) =>
            {
                var counterService = sp.GetRequiredService<ICounterService>();
                if (!counterService.TryFindCounter(counterName))
                {
                    await counterService.CreateNewCounterAsync(counterName, prefix, suffix, initialValue, maxValue);
                }
            };

            BackgroundTaskQueue.Instance.QueueBackgroundWorkItem(function);
            BackgroundTaskList.Instance.Add(function);
            return builder;

        }
    }
}
