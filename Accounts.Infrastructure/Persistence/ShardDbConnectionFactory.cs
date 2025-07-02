using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Infrastructure.Common.Tenant;

namespace Accounts.Infrastructure.Persistence;

public class ShardDbConnectionFactory(IConfiguration configuration, ITenantProvider tenantProvider ) : IShardDbConnectionFactory
{
    public IDbConnection CreateConnectionForTenant(string tenantKey)
    {
        var connStr = configuration.GetConnectionString(tenantKey)
                      ?? throw new InvalidOperationException($"Connection string not found for tenant '{tenantKey}'");

        Console.WriteLine($"🔗 Connecting to DB with tenantKey: {tenantKey}");
        Console.WriteLine($"🔗 Connection string: {connStr}");

        return new NpgsqlConnection(connStr);
    }

    public IDbConnection CreateConnection()
    {
        var tenantKey = tenantProvider.GetTenantKey();

        Console.WriteLine($"🔍 TenantKey = {tenantKey}");

        return CreateConnectionForTenant(tenantKey);
    }

    public IDbConnection CreateDefaultConnection()
    {
        var connStr = configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("DefaultConnection not found");

        return new NpgsqlConnection(connStr);
    }
}