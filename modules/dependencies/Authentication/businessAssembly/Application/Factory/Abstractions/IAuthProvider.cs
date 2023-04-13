using System.Collections.Generic;
using System.Threading.Tasks;
using Transversals.Business.Application.Abstractions.DataObjects;
using Transversals.Business.Authentication.Application.AuthenticationNeos;

namespace Transversals.Business.Authentication.Application.Factory.Abstractions
{
    /// <summary>
    /// Interface for saving option events
    /// </summary>
    public interface IAuthProvider
    {
        Task<UserAuthenticationResult[]> RegisterUsersAsync(IEnumerable<UserAuthentication> userList);
        Task<IEnumerable<UserAuthentication>> GetAvailableUsersAsync();
    }
}
