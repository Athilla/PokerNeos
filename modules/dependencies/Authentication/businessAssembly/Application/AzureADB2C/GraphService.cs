using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Transversals.Business.Authentication.Application.AzureADB2C.Configuration;

namespace Transversals.Business.Authentication.Application.AzureADB2C
{
    /// <summary>
    /// Class GraphService provide an interface to graphClientService.
    /// </summary>
    public class GraphService : IGraphService
    {
        private readonly AzureB2COption _azureOptions;
        private readonly GraphServiceClient _graphClient;

        public GraphServiceClient Client => _graphClient;
        /// <summary>
        /// Gets the B2C extension application client identifier.
        /// </summary>
        /// <value>
        /// The B2C extension application client identifier.
        /// </value>
        public string? B2cExtensionAppClientId => _azureOptions.B2cExtensionAppClientId;
        /// <summary>
        /// Gets the name of the tenant.
        /// </summary>
        /// <value>
        /// The name of the tenant.
        /// </value>
        public string? TenantName => _azureOptions.TenantName;

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        public EmailContent EmailContent => _azureOptions.EmailContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public GraphService(IConfiguration configuration)
        {
            _azureOptions = configuration.GetSection("AzureB2COption").Get<AzureB2COption>();
            ClientSecretCredential authProvider = new ClientSecretCredential(
                                                        _azureOptions.TenantId,
                                                        _azureOptions.ClientId,
                                                        _azureOptions.ClientSecretKey);
            this._graphClient = new GraphServiceClient(authProvider);

        }


    }
}
