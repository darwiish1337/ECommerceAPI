using System.Data;
using Accounts.Application.DTOs;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Domain.Enums;
using Dapper;
using Infrastructure.Common.Tenant;

namespace Accounts.Infrastructure.Persistence.Queries;

public class UserVerificationQueries(IShardDbConnectionFactory connectionFactory, ITenantProvider tenantProvider) : IUserVerificationQueries
{
    private IDbConnection CreateConnection()
    {
        var tenantKey = tenantProvider.GetTenantKey();
        return connectionFactory.CreateConnectionForTenant(tenantKey);
    }

    public async Task<UserVerificationDto?> GetLatestUnexpiredAsync(Guid userId, VerificationType type, CancellationToken cancellationToken)
    {
        using var connection = CreateConnection();
        
        const string sql = """
                               SELECT "Id", "UserId", "Code", "ExpiresAt", "IsUsed", "Type"
                               FROM "UserVerifications"
                               WHERE "UserId" = @UserId AND "Type" = @Type AND "IsUsed" = FALSE AND "ExpiresAt" > NOW()
                               ORDER BY "ExpiresAt" DESC
                               LIMIT 1;
                           """;

        return await connection.QueryFirstOrDefaultAsync<UserVerificationDto>(sql, new { UserId = userId, Type = type });
    }
}
