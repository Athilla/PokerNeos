using FluentResults;
using GroupeIsa.Neos.Application.MultiTenant;
using GroupeIsa.Neos.Shared.MultiTenant;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Transversals.Business.Core.Domain.HostedServices;

namespace Transversals.Business.Core.Domain.Migration
{

    [ExcludeFromCodeCoverage]
    internal class MTTenantDatabaseMigrationInterceptor : ITenantDatabaseMigrationInterceptor
    {
        private readonly ILogger<MTTenantDatabaseMigrationInterceptor> _logger;
        private readonly IServiceProvider _serviceProvider;

        public MTTenantDatabaseMigrationInterceptor(ILogger<MTTenantDatabaseMigrationInterceptor> logger,
                                                    IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task<Result> OnTenantDatabaseMigratedAsync(TenantDatabaseMigratedEventData tenantDatabaseMigratedEventData)
        {
            //Gestion des options et compteurs predefinit
            int nb = BackgroundTaskList.Instance.Count;
            for (int i = 0; i < nb; i++)
            {
                var data = BackgroundTaskList.Instance.Get(i);
                if (data != null)
                {
                    try
                    {
                        _logger.LogDebug("Treating data...");
                        var task = data.Invoke(_serviceProvider);
                        await task.WaitAsync(new TimeSpan(0, 0, 1));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error when invoking lambda");
                        return Result.Fail($"Failed to initialize tenant");
                    }
                }
            }
            return Result.Ok();
        }
    }
}
