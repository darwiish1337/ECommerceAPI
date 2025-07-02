using System.Threading;

namespace Infrastructure.Common.Tenant;

public class HeaderTenantProvider : ITenantProvider
{
    private static readonly AsyncLocal<string?> _currentTenant = new();

    public string GetTenantKey()
        => _currentTenant.Value ?? throw new InvalidOperationException("Tenant is not set.");

    public void SetTenant(string tenantKey)
        => _currentTenant.Value = tenantKey;

    // اختياري لو عاوز تستخدمه كـ Property
    public string CurrentTenant => GetTenantKey();
}