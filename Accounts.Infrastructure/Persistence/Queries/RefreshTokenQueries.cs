using System.Data;
using Accounts.Application.DTOs;
using Accounts.Application.Interfaces.Persistence.Queries;
using Dapper;
using Infrastructure.Common.Tenant;

namespace Accounts.Infrastructure.Persistence.Queries;

public class RefreshTokenQueries(IShardDbConnectionFactory connectionFactory, ITenantProvider tenantProvider ) : IRefreshTokenQueries
{
    private IDbConnection CreateConnection()
    {
        var tenantKey = tenantProvider.GetTenantKey();
        return connectionFactory.CreateConnectionForTenant(tenantKey);
    }

    public async Task<RefreshTokenDto?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        const string sql = """
                               SELECT Id, UserId, Token, ExpiresAt,
                                      RevokedAt IS NULL AND ExpiresAt > NOW() AS IsActive
                               FROM RefreshTokens
                               WHERE Token = @Token
                               LIMIT 1;
                           """;

        using var connection = CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<RefreshTokenDto>(sql, new { Token = token });
    }
}