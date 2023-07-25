using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupeIsa.Neos.Application;
using GroupeIsa.Neos.Application.MultiTenant;
using GroupeIsa.Neos.Domain.Persistence;
using GroupeIsa.Neos.Shared.Logging;
using GroupeIsa.Neos.Shared.MultiTenant;
using Microsoft.Extensions.Configuration;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.UserPermissions.Application
{
    internal class TenantResolvedInterceptor : ITenantResolvedInterceptor
    {
        private readonly IUserInfo? _user;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly INeosLogger<TenantResolvedInterceptor> _logger;
        private readonly IApplicationInfo _applicationInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantResolvedInterceptor"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="applicationInfo"></param>
        public TenantResolvedInterceptor(IUserInfoAccessor user,
                               IUserAccountRepository userAccountRepository,
                               IUnitOfWork unitOfWork,
                               IConfiguration configuration,
                               INeosLogger<TenantResolvedInterceptor> logger,
                               IApplicationInfo applicationInfo)
        {
            _user = user.User;
            _userAccountRepository = userAccountRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _applicationInfo = applicationInfo;
        }

        public async Task<OnTenantResolvedResult> OnTenantResolvedAsync(NeosTenantInfo? tenant, OnTenantResolvedResult? previousTenantResolvedResult)
        {
            _logger.LogDebug("We receive on demand for authenticate user : {email}", _user?.Email);

            if (_user == null)
            {
                return new OnAuthenticatedResult(false);
            }

            string? userEmail = _user.Email?.ToLower();
            if (userEmail == null)
            {
                return new OnAuthenticatedResult(false);
            }

            //recherche des comptes utilisateur existants
            UserAccount? userAccount = _userAccountRepository.GetQuery().SingleOrDefault(u => u.Email.ToLower() == userEmail);

            if (userAccount != null && userAccount.IsActive
                && (userAccount.ExpirationDate ?? System.DateTime.Now.Date) >= System.DateTime.Now.Date)
            {
                userAccount.LastConnection = System.DateTime.Now;
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("We fount user : {email} => authorized", userEmail);

                var context = new Dictionary<string, object>()
                {
                    ["UserFirstName"] = userAccount.FirstName,
                    ["UserLastName"] = userAccount.LastName,
                };

                if (_configuration["AuthenticationMode"] is string authenticationMode)
                {
                    context["AuthProvider"] = authenticationMode;
                }

                return new OnAuthenticatedResult(true,
                                                 context,
                                                 theme: _applicationInfo.Themes.FirstOrDefault(t => t == "001"),
                                                 applicationCulture: userAccount.DisplayCultureCode,
                                                 inputCultures: userAccount.InputCultureCodes);
            }

            //check on bypass list
            var section = _configuration.GetSection("AuthorizedEmails");
            IEnumerable<string>? authorizedEmails = section.Get<string[]>();
            
            if (authorizedEmails != null && authorizedEmails.Contains(userEmail))
            {
                //passage d'un dictionnaire vide pour l'initialisation de l'application context
                _logger.LogInformation("We fount user : {userEmail} => authorized (bypass)", userEmail);

                Dictionary<string, object>? context = new();
                if (_configuration["AuthenticationMode"] is string authenticationMode)
                {
                    context["AuthProvider"] = authenticationMode;
                }

                return new OnAuthenticatedResult(true, context, theme: _applicationInfo.Themes.FirstOrDefault(t => t == "001"));

            }

            return new OnAuthenticatedResult(false);
        }
    }
}
