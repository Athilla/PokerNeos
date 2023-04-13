using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Application.Abstractions.Methods;
using Transversals.Business.Authentication.Application.Factory;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.Authentication.Application.Methods
{
    /// <summary>
    /// Represents GetAvailableUsersFromAuthenticationServer method.
    /// </summary>
    public class GetAvailableUsersFromAuthenticationServer : IGetAvailableUsersFromAuthenticationServer
    {
        private readonly IEnumerable<IFactoryAuthProvider> _factoryAuthProvider;
        private readonly IUserAccountRepository _userAccountRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailableUsersFromAuthenticationServer"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public GetAvailableUsersFromAuthenticationServer(IEnumerable<IFactoryAuthProvider> factoryAuthProvider,
                        IUserAccountRepository userAccountRepository)
        {
            _factoryAuthProvider = factoryAuthProvider;
            _userAccountRepository = userAccountRepository;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserAuthentication>> ExecuteAsync()
        {
            List<UserAuthentication> result = new List<UserAuthentication>();
            foreach (var factoryAuthProvider in _factoryAuthProvider)
            {
                var provider = factoryAuthProvider.GetProvider();
                if (provider != null)
                    result.AddRange(await provider.GetAvailableUsersAsync());
            }
            //on filtre on exclut ce qui sont deja en base.
            var listUsers = _userAccountRepository.GetQuery().ToList();
            List<UserAuthentication> resultFiltered = new List<UserAuthentication>();
            foreach (var user in result)
            {
                if (listUsers.Any(lu => lu.Email.ToLower() == user.Email.ToLower()))
                {
                    continue;
                }
                resultFiltered.Add(user);
            }

            return resultFiltered;
        }
    }
}