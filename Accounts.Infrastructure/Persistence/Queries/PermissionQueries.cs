using System.Data;
using Accounts.Application.DTOs;
using Accounts.Application.Interfaces.Persistence.Queries;
using Dapper;
using Infrastructure.Common.Tenant;
using Microsoft.Extensions.Logging;

namespace Accounts.Infrastructure.Persistence.Queries;

public class PermissionQueries(IShardDbConnectionFactory connectionFactory, ITenantProvider tenantProvider,
    ILogger<PermissionQueries> logger) : IPermissionQueries
{
    private IDbConnection CreateConnection()
    {
        var tenantKey = tenantProvider.GetTenantKey();
        logger.LogInformation("🔑 Using tenant: {Tenant}", tenantKey);
        return connectionFactory.CreateConnectionForTenant(tenantKey);
    }

    public async Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Name FROM Permissions";

        using var connection = CreateConnection();
        logger.LogInformation("📥 Fetching all permissions");

        var permissions = await connection.QueryAsync<PermissionDto>(sql);
        return permissions.ToList();
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Name FROM Permissions WHERE Id = @Id";

        using var connection = CreateConnection();
        logger.LogInformation("📥 Fetching permission by Id: {Id}", id);

        return await connection.QueryFirstOrDefaultAsync<PermissionDto>(sql, new { Id = id });
    }
    
    public async Task<List<PermissionDto>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
        SELECT p.Id, p.Name
        FROM Permissions p
        INNER JOIN RolePermissions rp ON rp.PermissionsId = p.Id
        WHERE rp.RolesId = @RoleId;
    ";

        using var connection = CreateConnection();

        var result = await connection.QueryAsync<PermissionDto>(sql, new { RoleId = roleId });
        return result.ToList();
    }
}