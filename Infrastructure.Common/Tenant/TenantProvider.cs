namespace Infrastructure.Common.Tenant;

public class TenantProvider : ITenantProvider
{
    private static readonly AsyncLocal<string?> _currentTenant = new();

    public string GetTenantKey()
        => _currentTenant.Value ?? throw new InvalidOperationException("Tenant is not set.");

    public void SetTenant(string tenantKey)
        => _currentTenant.Value = tenantKey;

    public string CurrentTenant => GetTenantKey(); // optional
}