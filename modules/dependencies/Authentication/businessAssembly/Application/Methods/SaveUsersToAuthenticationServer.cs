using GroupeIsa.Neos.Domain.Persistence;
using GroupeIsa.Neos.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Authentication.Application.AuthenticationNeos;
using Transversals.Business.Authentication.Application.Factory;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.Domain.Properties;
using static Transversals.Business.Authentication.Application.AuthenticationNeos.UserAuthenticationResult;

namespace Transversals.Business.Authentication.Application.Methods
{
    /// <summary>
    /// Represents SaveUsersToAuthenticationServer method.
    /// </summary>
    public class SaveUsersToAuthenticationServer : ISaveUsersToAuthenticationServer
    {
        private readonly INeosLogger<ISaveUsersToAuthenticationServer> _logger;
        private readonly IEnumerable<IFactoryAuthProvider> _factoryAuthProvider;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveUsersToAuthenticationServer"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public SaveUsersToAuthenticationServer(
            INeosLogger<ISaveUsersToAuthenticationServer> logger,
            IEnumerable<IFactoryAuthProvider> factoryAuthProvider,
            IUserAccountRepository userAccountRepository,
            IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _factoryAuthProvider = factoryAuthProvider;
            _userAccountRepository = userAccountRepository;
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public async Task<ServerMethodResult> ExecuteAsync(UserAuthentication[] userInformationList)
        {
            if (!userInformationList.Any())
            {
                return new ServerMethodResult()
                {
                    Message = Resources.Authentication.SaveUsersToAuthenticationServerFailed,
                };
            }
            
            List<UserAuthenticationResult> result = new List<UserAuthenticationResult>();
            foreach (var factoryAuthProvider in _factoryAuthProvider)
            {
                var provider = factoryAuthProvider.GetProvider();
                if(provider != null)
                    result.AddRange(await provider.RegisterUsersAsync(userInformationList));
            }

            if (!result.Any())
            {
                return new ServerMethodResult()
                {
                    Message = Resources.Authentication.SaveUsersToAuthenticationServerFailed,
                };
            }

            //récupération des utilisateurs en échec
            List<UserAuthenticationResult> failedsUser = new();
            if (!result.Select(r => r.Name).SequenceEqual(userInformationList.Select(u => u.Name)))
            {
                IEnumerable<UserAuthenticationResult> namesNotInResult = userInformationList.Where(u => !result.Select(r => r.Name).Contains(u.Name))
                                                                        .Select(u => new UserAuthenticationResult() { Name = u.Name, State = StateType.Failed});
                failedsUser.AddRange(namesNotInResult);
            }

            if (result.Any(r => r.State != UserAuthenticationResult.StateType.Succeeded
                             && r.State != UserAuthenticationResult.StateType.AlreadyCreated))
            {
                failedsUser.AddRange(result
                           .Where(r => r.State != UserAuthenticationResult.StateType.Succeeded
                                    && r.State != UserAuthenticationResult.StateType.AlreadyCreated));
            }

            IEnumerable<IGrouping<string?, UserAuthenticationResult>> groupedResult = result.GroupBy(r => r.Name);
            List<UserAccount> users = _userAccountRepository.GetQuery()
                                                            .Where(c => groupedResult.Select(g => g.Key).Contains(c.Login))
                                                            .ToList();

            //mise à jour du statut "Synchronized" pour les utilisateurs enregistrés
            foreach (UserAccount user in users)
            {
                StateType? state = groupedResult.SingleOrDefault(g => g.Key == user.Login)?.FirstOrDefault()?.State;
                if (state == UserAuthenticationResult.StateType.Succeeded
                    || state == UserAuthenticationResult.StateType.AlreadyCreated)
                {
                    user.Synchronized = true;
                }
            }

            //sauvegarde des utilisateurs si modification
            if (users.Any(u => u.Synchronized))
            {
                await _unitOfWork.SaveAsync();
            }

            return new ServerMethodResult()
            {
                Message = GetErrorMessage(failedsUser),
            };
        }

        /// <summary>
        /// Retourne un texte d'erreur pour la liste d'utilisateurs non synchronisés.
        /// </summary>
        /// <param name="failedsUserNames">Login des utilisateurs en erreur</param>
        /// <returns>Message d'erreur ou <c>null</c></returns>
        private static string? GetErrorMessage(List<UserAuthenticationResult> failedsUser)
        {
            if (!failedsUser.Any())
            {
                return null;
            }

            string message = string.Join($"{Environment.NewLine}- ", failedsUser.Distinct().Select(e => ParseError(e)));
            return string.Format(Resources.Authentication.SaveUsersToAuthenticationServerFailedForUsers, message);
        }

        private static string? ParseError(UserAuthenticationResult userAuthResult)
        {
            switch (userAuthResult.State)
            {
                case UserAuthenticationResult.StateType.AlreadyCreatedAndDifferentEmail:
                    return $"{userAuthResult.Name} {Resources.Authentication.AuthNeosErrorLoginAlreadyUsedEmail}";
                default:
                    return userAuthResult.Name;
            }
        }
    }
}