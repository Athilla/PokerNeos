using GroupeIsa.Neos.Domain.Exceptions;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Authentication.Application.AuthenticationNeos;
using Transversals.Business.Authentication.Application.Factory;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Properties;
using static Transversals.Business.Authentication.Application.AuthenticationNeos.UserAuthenticationResult;

namespace Transversals.Business.Authentication.Application.UserAccountDomainEventRules
{
    /// <summary>
    /// Represents Saving event rule.
    /// </summary>
    public class Saving : ISavingRule<UserAccount>
    {
        private readonly ISavingRule<UserAccount> _baseRule;
        private readonly IEnumerable<IFactoryAuthProvider> _factoryAuthProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saving"/> class.
        /// </summary>
        /// <param name="baseRule">BaseRule.</param>
        /// <param name="logger">Logger.</param>
        public Saving(ISavingRule<UserAccount> baseRule,
            IEnumerable<IFactoryAuthProvider> factoryAuthProvider)
        {
            _baseRule = baseRule;
            _factoryAuthProvider = factoryAuthProvider;
        }

        /// <inheritdoc/>
        public async Task OnSavingAsync(ISavingRuleArguments<UserAccount> args)
        {
            // Calling the base code. You can add code before and/or after or remove the call but you cannot remove the dependency.
            await _baseRule.OnSavingAsync(args);

            IEnumerable<UserAuthentication> userList = args.CreatedItems.Select(item => new UserAuthentication()
            {
                Name = item.Login,
                Email = item.Email,
                Language = "French",
                FirstName = item.FirstName,
                LastName = item.LastName
            });

            await PostUserToAuthenticationServerAsync(args.CreatedItems, userList.ToList());
        }

        /// <summary>
        /// Posts the user to authentication server.
        /// </summary>
        /// <param name="createdItems">The created items.</param>
        /// <param name="userList">The user list.</param>
        private async Task PostUserToAuthenticationServerAsync(IReadOnlyList<UserAccount> createdItems, IEnumerable<UserAuthentication> userList)
        {
            if (userList.Any())
            {
                List<UserAuthenticationResult> result = new List<UserAuthenticationResult>();
                foreach (var factoryAuthProvider in _factoryAuthProvider)
                {
                    var provider = factoryAuthProvider.GetProvider();
                    if (provider != null)
                        result.AddRange(await provider.RegisterUsersAsync(userList));
                }

                if (!result.Any())
                {
                    throw new BusinessException(Resources.Authentication.SaveUsersToAuthenticationServerFailed);
                }

                //récupération des utilisateurs en échec
                List<UserAuthenticationResult> failedsUser = new();
                if (!result.Select(r => r.Name).SequenceEqual(userList.Select(u => u.Name)))
                {
                    IEnumerable<UserAuthenticationResult> namesNotInResult = userList.Where(u => !result.Select(r => r.Name).Contains(u.Name))
                                                                            .Select(u => new UserAuthenticationResult() { Name = u.Name, State = StateType.Failed });
                    failedsUser.AddRange(namesNotInResult);
                }

                if (result.Any(r => r.State != UserAuthenticationResult.StateType.Succeeded
                                 && r.State != UserAuthenticationResult.StateType.AlreadyCreated))
                {
                    failedsUser.AddRange(result
                               .Where(r => r.State != UserAuthenticationResult.StateType.Succeeded
                                        && r.State != UserAuthenticationResult.StateType.AlreadyCreated));
                }

                if(failedsUser.Any())
                {
                    throw new BusinessException(GetErrorMessage(failedsUser));
                }

                foreach (UserAuthenticationResult userAuthenticationResult in result)
                {
                    if (userAuthenticationResult.State == UserAuthenticationResult.StateType.Succeeded
                        || userAuthenticationResult.State == UserAuthenticationResult.StateType.AlreadyCreated)
                    {
                        createdItems.Where(c => c.Login == userAuthenticationResult.Name)
                                    .ToList()
                                    .ForEach(c => c.Synchronized = true);
                    }
                }
            }
        }

        /// <summary>
        /// Retourne un texte d'erreur pour la liste d'utilisateurs non synchronisés
        /// </summary>
        /// <param name="failedsUserNames">Login des utilisateurs en erreur</param>
        /// <returns>Message d'erreur ou <c>null</c></returns>
        private static string GetErrorMessage(List<UserAuthenticationResult> failedsUser)
        {
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