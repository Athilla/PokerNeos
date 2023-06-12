using GroupeIsa.Neos.Application;
using GroupeIsa.Neos.Application.Metadata;
using GroupeIsa.Neos.Domain.Persistence;
using GroupeIsa.Neos.Shared.MultiTenant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.UserPermissions.Application.Methods
{
    /// <summary>
    /// Represents OnTenantResolved method.
    /// </summary>
    public class OnTenantResolved : IOnTenantResolved
    {
        private readonly IUserInfo _user;
        private readonly IConfiguration _configuration;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OnTenantResolved> _logger;
        private readonly IApplicationInfo _applicationInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnTenantResolved"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="applicationInfo"></param>
        public OnTenantResolved(IUserInfoAccessor user,
                               IUserAccountRepository userAccountRepository,
                               IUnitOfWork unitOfWork,
                               IConfiguration configuration,
                               ILogger<OnTenantResolved> logger,
                               IApplicationInfo applicationInfo)
        {
            _user = user.User!;
            _userAccountRepository = userAccountRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _applicationInfo = applicationInfo;
        }

        /// <inheritdoc/>
        public async Task<OnTenantResolvedResult> ExecuteAsync()
        {
            _logger.LogDebug("We receive on demand for authenticate user : {email}", _user.Email);

            var userEmail = _user.Email?.ToLower() ?? string.Empty;
            //recherche des comptes utilisateur existants
            UserAccount? userAccount = _userAccountRepository.GetQuery().Where(u => u.Email == userEmail).SingleOrDefault();

            if (userAccount != null && userAccount.IsActive
                && (userAccount.ExpirationDate ?? System.DateTime.Now.Date) >= System.DateTime.Now.Date)
            {
                userAccount.LastConnection = System.DateTime.Now;
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("We fount user : {email} => authorized", _user.Email);

                var context = new Dictionary<string, object>()
                {
                    ["UserFirstName"] = userAccount.FirstName,
                    ["UserLastName"] = userAccount.LastName,
                };

                if (_configuration["AuthenticationMode"] is string authenticationMode)
                {
                    context["AuthProvider"] = authenticationMode;
                }

                return new OnAuthenticatedResult(true, context, theme: _applicationInfo.Themes.FirstOrDefault(t => t == "001"));
            }

            //check on bypass list
            var section = _configuration.GetSection("AuthorizedEmails");
            IEnumerable<string>? authorizedEmails = section.Get<string[]>();
            if (authorizedEmails == null)
            {
                return new OnAuthenticatedResult(false);
            }
            if (authorizedEmails.Contains(userEmail))
            {
                //passage d'un dictionnaire vide pour l'initialisation de l'application context
                _logger.LogInformation("We fount user : {userEmail} => authorized (bypass)", userEmail);

                Dictionary<string, object>? context;
                if (_configuration["AuthenticationMode"] is string authenticationMode)
                {
                    context = new Dictionary<string, object>()
                    {
                        ["AuthProvider"] = authenticationMode
                    };
                }
                else
                {
                    context = null;
                }

                return new OnAuthenticatedResult(true, context, theme: _applicationInfo.Themes.FirstOrDefault(t => t == "001"));
            }

            return new OnAuthenticatedResult(false);
        }
    }
}
