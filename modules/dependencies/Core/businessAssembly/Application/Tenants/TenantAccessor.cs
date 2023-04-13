using GroupeIsa.Neos.Application.MultiTenant;
using GroupeIsa.Neos.Shared.MultiTenant;

namespace Transversals.Business.Core.Application.Tenants
{
    public class TenantAccessor : ITenantAccessor
    {

        public TenantAccessor(ITenants tenants)
        {
            TenantsInfo = tenants.GetCurrentTenant();
            MultitenancyEnabled = tenants.MultitenancyEnabled;
        }

        public NeosTenantInfo? TenantsInfo { get; }
        public bool MultitenancyEnabled { get; }
    }
}
