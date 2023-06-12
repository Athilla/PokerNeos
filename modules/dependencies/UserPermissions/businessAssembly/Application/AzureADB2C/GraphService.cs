using System;
using System.Runtime.Serialization;
using Azure.Identity;
using GroupeIsa.Neos.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Transversals.Business.UserPermissions.Application.AzureADB2C.Configuration;

namespace Transversals.Business.UserPermissions.Application.AzureADB2C
{
    /// <summary>
    /// Class GraphService provide an interface to graphClientService.
    /// </summary>
    public class GraphService : IGraphService
    {
        private readonly AzureB2COption? _azureOptions;
        private readonly GraphServiceClient? _graphClient;

        public GraphServiceClient Client => _graphClient ?? throw new AzureB2COptionConfigurationException();
        /// <summary>
        /// Gets the B2C extension application client identifier.
        /// </summary>
        /// <value>
        /// The B2C extension application client identifier.
        /// </value>
        public string? B2cExtensionAppClientId => AzureOptions.B2cExtensionAppClientId;
        /// <summary>
        /// Gets the name of the tenant.
        /// </summary>
        /// <value>
        /// The name of the tenant.
        /// </value>
        public string? TenantName => AzureOptions.TenantName;

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        public EmailContent EmailContent => AzureOptions.EmailContent;

        private AzureB2COption AzureOptions => _azureOptions ?? throw new AzureB2COptionConfigurationException();

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public GraphService(IConfiguration configuration)
        {
            _azureOptions = configuration.GetSection("AzureB2COption").Get<AzureB2COption>();
            if (_azureOptions != null)
            {
                ClientSecretCredential authProvider = new(
                                                            _azureOptions.TenantId,
                                                            _azureOptions.ClientId,
                                                            _azureOptions.ClientSecretKey);
                this._graphClient = new GraphServiceClient(authProvider);
            }
        }

        [Serializable]
        public class AzureB2COptionConfigurationException : BusinessException
        {
            public AzureB2COptionConfigurationException()
                : base("AzureB2COption configuration not found")
            {
            }

            // Without this constructor, deserialization will fail
            protected AzureB2COptionConfigurationException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
