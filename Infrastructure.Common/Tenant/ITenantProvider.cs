namespace Infrastructure.Common.Tenant;

public interface ITenantProvider
{
    string GetTenantKey();
    
    void SetTenant(string tenantKey); // ✨ Add this
}

