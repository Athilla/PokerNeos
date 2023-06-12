using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Transversals.Business.UserPermissions.Application.AuthenticationNeos;
using Transversals.Business.UserPermissions.Application.Factory.Abstractions;
using Transversals.Business.UserPermissions.Application.AzureADB2C;

namespace Transversals.Business.UserPermissions.Application.Factory
{
    public class FactoryAuthProvider : IFactoryAuthProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public FactoryAuthProvider(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }
        public IAuthProvider? GetProvider()
        {
            return _configuration["AuthenticationMode"] switch
            {
                "Neos" => _serviceProvider.GetService<AuthenticationNeosClient>(),
                "AzureB2C" => _serviceProvider.GetService<AzureB2CClient>(),
                _ => null,
            };
        }
    }
}
