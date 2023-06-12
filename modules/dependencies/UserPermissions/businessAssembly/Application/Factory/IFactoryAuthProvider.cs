using Transversals.Business.UserPermissions.Application.Factory.Abstractions;

namespace Transversals.Business.UserPermissions.Application.Factory
{
    public interface IFactoryAuthProvider
    {
        IAuthProvider? GetProvider();
    }
}