using System.Data;
using Accounts.Application.DTOs;
using Accounts.Application.Interfaces.Persistence.Queries;
using Dapper;
using Infrastructure.Common.Tenant;
using Microsoft.Extensions.Logging;

namespace Accounts.Infrastructure.Persistence.Queries;

public class RoleQueries(IShardDbConnectionFactory connectionFactory, ITenantProvider tenantProvider,
    ILogger<RoleQueries> logger) : IRoleQueries
{
    private IDbConnection CreateConnection()
    {
        var tenantKey = tenantProvider.GetTenantKey();
        logger.LogInformation("🔑 Using tenant: {Tenant}", tenantKey);
        return connectionFactory.CreateConnectionForTenant(tenantKey);
    }

    public async Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Name FROM Roles";

        using var connection = CreateConnection();
        logger.LogInformation("📥 Fetching all roles");

        var roles = await connection.QueryAsync<RoleDto>(sql);
        return roles.ToList();
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Name FROM Roles WHERE Id = @Id";

        using var connection = CreateConnection();
        logger.LogInformation("📥 Fetching role by Id: {Id}", id);

        return await connection.QueryFirstOrDefaultAsync<RoleDto>(sql, new { Id = id });
    }

    public async Task<RoleWithPermissionsDto?> GetWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT r.Id, r.Name, p.Name AS PermissionName
            FROM Roles r
            LEFT JOIN RolePermissions rp ON rp.RoleId = r.Id
            LEFT JOIN Permissions p ON p.Id = rp.PermissionId
            WHERE r.Id = @RoleId;
        ";

        using var connection = CreateConnection();
        logger.LogInformation("📥 Fetching role with permissions for Id: {RoleId}", roleId);

        var lookup = new Dictionary<Guid, RoleWithPermissionsDto>();

        await connection.QueryAsync<RoleWithPermissionsDto, string, RoleWithPermissionsDto>(
            sql,
            (role, permissionName) =>
            {
                if (!lookup.TryGetValue(role.Id, out var dto))
                {
                    dto = role;
                    dto.Permissions = new List<string>();
                    lookup.Add(role.Id, dto);
                }

                if (!string.IsNullOrWhiteSpace(permissionName))
                    dto.Permissions.Add(permissionName);

                return dto;
            },
            new { RoleId = roleId },
            splitOn: "PermissionName"
        );

        return lookup.Values.FirstOrDefault();
    }
    
    public async Task<List<RoleDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT id, name FROM roles WHERE id = ANY(@Ids)";
        using var connection = CreateConnection();
    
        var result = await connection.QueryAsync<RoleDto>(sql, new { Ids = ids });
        return result.ToList();
    }

}