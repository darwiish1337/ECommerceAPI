using System.Data;

namespace Accounts.Infrastructure.Persistence;

public interface IShardDbConnectionFactory
{
    IDbConnection CreateConnectionForTenant(string tenantKey);

    IDbConnection CreateDefaultConnection(); 
}