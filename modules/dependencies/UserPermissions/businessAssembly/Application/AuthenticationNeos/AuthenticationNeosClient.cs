using Flurl.Http;
using Flurl.Http.Configuration;
using GroupeIsa.Neos.Shared.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.UserPermissions.Application.Factory.Abstractions;

namespace Transversals.Business.UserPermissions.Application.AuthenticationNeos
{
    public class AuthenticationNeosClient : IAuthProvider
    {
        private readonly IFlurlClient _flurlClient;
        private readonly INeosLogger<AuthenticationNeosClient> _logger;
        private readonly IConfiguration _configuration;
        private const string AuthenticationEndpoint = "/api/users";
        private const string AuthenticationToken = "97qvFExgqhkgAp86aGSFzY4f5kXeWs8r";

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationNeosClient"/> class.
        /// </summary>
        /// <param name="clusterFactory">The cluster factory.</param>
        /// <param name="flurlClient">The flurl client.</param>
        public AuthenticationNeosClient(INeosLogger<AuthenticationNeosClient> logger,
                                          IConfiguration configuration,
                                          IFlurlClientFactory flurlClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _flurlClient = flurlClientFactory
                .Get(string.Empty)
                .Configure(settings => settings.OnError = HandleFlurlError);
        }

        /// <summary>
        /// Register new user to Neos authentication server.
        /// </summary>
        /// <param name="userList">The user list.</param>
        public async Task<UserAuthenticationResult[]> RegisterUsersAsync(IEnumerable<UserAuthentication> userList)
        {
            string? baseUrl = _configuration["Authentication:Authority"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                return Array.Empty<UserAuthenticationResult>();
            }

            _flurlClient.BaseUrl = baseUrl;

            IFlurlRequest req = _flurlClient.Request(AuthenticationEndpoint);

            UserAuthenticationResult[] result = await req
                .AllowHttpStatus(HttpStatusCode.OK)
                .WithOAuthBearerToken(AuthenticationToken)
                .PostJsonAsync(userList)
                .ReceiveJson<UserAuthenticationResult[]>();

            _logger.LogInformation(req.Url);

            return result;
        }

        /// <summary>
        /// Handles the flurl error.
        /// </summary>
        /// <param name="call">The call.</param>
        private void HandleFlurlError(FlurlCall flurlCall)
        {
            _logger.LogError(flurlCall.Exception.Message);
            flurlCall.ExceptionHandled = true;
        }

        /// <summary>
        /// Gets the available user.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserAuthentication>> GetAvailableUsersAsync()
        {
            return await Task.FromResult(new List<UserAuthentication>());
        }
    }
}
