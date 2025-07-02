using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Infrastructure.Common.Tenant;

namespace Accounts.Infrastructure.Persistence;

public class ShardDbContextFactory(IConfiguration configuration, ITenantProvider tenantProvider ) : IShardDbContextFactory
{
    public AccountsDbContext CreateDbContextForTenant(string tenantKey)
    {
        var connectionString = configuration.GetConnectionString(tenantKey)
                               ?? throw new InvalidOperationException($"Connection string not found for tenant '{tenantKey}'");

        var optionsBuilder = new DbContextOptionsBuilder<AccountsDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AccountsDbContext(optionsBuilder.Options);
    }

    public AccountsDbContext CreateDbContext()
    {
        var tenantKey = tenantProvider.GetTenantKey();
        return CreateDbContextForTenant(tenantKey);
    }
}