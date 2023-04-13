using Transversals.Business.Authentication.Application.Factory.Abstractions;

namespace Transversals.Business.Authentication.Application.Factory
{
    public interface IFactoryAuthProvider
    {
        IAuthProvider? GetProvider();
    }
}