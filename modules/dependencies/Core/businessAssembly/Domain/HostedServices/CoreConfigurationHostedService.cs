using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Transversals.Business.Core.Application.Tenants;

namespace Transversals.Business.Core.Domain.HostedServices
{
    /// <summary>
    /// CoreConfiguration Background service
    /// for checking initial configuration
    /// cf : https://andrewlock.net/finding-the-urls-of-an-aspnetcore-app-from-a-hosted-service-in-dotnet-6/
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
    public class CoreConfigurationHostedService : BackgroundService
    {
        private readonly ILogger<CoreConfigurationHostedService> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IServiceProvider _serviceProvider;

        public CoreConfigurationHostedService(ILogger<CoreConfigurationHostedService> logger, IHostApplicationLifetime lifetime, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _lifetime = lifetime;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!await WaitForAppStartupAsync(_lifetime, stoppingToken))
            {
                return;
            }
            using var sc = _serviceProvider.CreateScope();
            ITenantAccessor? _tenantAccessor = sc.ServiceProvider.GetService<ITenantAccessor>();
            if (_tenantAccessor != null && _tenantAccessor.MultitenancyEnabled)
            {
                _logger.LogWarning("Runing in tenant mode, not need worker");
                return;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = await BackgroundTaskQueue.Instance.DequeueAsync(stoppingToken);
                if (data != null)
                {
                    try
                    {
                        Console.WriteLine("Treating data...");
                        var task = data.Invoke(sc.ServiceProvider);
                        await task.WaitAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error when invoking lambda");
                    }
                }
            }
        }

        private static async Task<bool> WaitForAppStartupAsync(IHostApplicationLifetime lifetime, CancellationToken cancellationToken)
        {
            var startedSource = new TaskCompletionSource();
            var cancelledSource = new TaskCompletionSource();
            Task completedTask = Task.CompletedTask;
            if (!cancellationToken.IsCancellationRequested)
            {

                using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
                using var reg2 = cancellationToken.Register(() => cancelledSource.SetResult());

                completedTask = await Task.WhenAny(
                startedSource.Task,
                cancelledSource.Task).ConfigureAwait(false);
            }

            // If the completed tasks was the "app started" task, return true, otherwise false
            return completedTask == startedSource.Task;
        }
    }
}
