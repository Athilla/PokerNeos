using GroupeIsa.Neos.Shared.MultiTenant;

namespace Transversals.Business.Core.Application.Tenants
{
    public interface ITenantAccessor
    {
        NeosTenantInfo? TenantsInfo { get; }
        bool MultitenancyEnabled { get; }
    }
}