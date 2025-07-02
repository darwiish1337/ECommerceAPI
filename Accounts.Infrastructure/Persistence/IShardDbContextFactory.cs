namespace Accounts.Infrastructure.Persistence;

public interface IShardDbContextFactory
{
    AccountsDbContext CreateDbContextForTenant(string tenantKey);
    
    AccountsDbContext CreateDbContext();
}