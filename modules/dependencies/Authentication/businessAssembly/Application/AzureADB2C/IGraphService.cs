using Microsoft.Graph;
using Transversals.Business.Authentication.Application.AzureADB2C.Configuration;

namespace Transversals.Business.Authentication.Application.AzureADB2C
{
    public interface IGraphService
    {
        string? B2cExtensionAppClientId { get; }
        GraphServiceClient Client { get; }
        string? TenantName { get; }
        EmailContent EmailContent { get; }
    }
}