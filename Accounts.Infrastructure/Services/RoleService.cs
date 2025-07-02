using Accounts.Application.DTOs;
using Accounts.Application.DTOs.Requests.Role;
using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Services;
using Accounts.Domain.Users;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Accounts.Infrastructure.Services;

public class RoleService(IUnitOfWork unitOfWork, IRoleQueries roleQueries, ILogger<RoleService> logger,
    IMapper mapper) : IRoleService
{
    public async Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all roles via Dapper");
        return await roleQueries.GetAllAsync(cancellationToken);
    }

    public async Task<RoleWithPermissionsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting role with permissions via Dapper for Id: {Id}", id);
        return await roleQueries.GetWithPermissionsAsync(id, cancellationToken);
    }

    public async Task<Guid> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new role: {Name}", request.Name);

        var role = new Role(Guid.NewGuid(), request.Name);
        await unitOfWork.Roles.AddAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Role created with Id: {Id}", role.Id);
        return role.Id;
    }

    public async Task UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating role: {Id}", request.Id);

        var roleDto = await roleQueries.GetByIdAsync(request.Id, cancellationToken);
        if (roleDto is null)
        {
            logger.LogWarning("Role not found: {Id}", request.Id);
            throw new Exception("Role not found");
        }

        var role = mapper.Map<Role>(roleDto);
        role.ChangeName(request.Name);

        await unitOfWork.Roles.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Role updated successfully: {Id}", request.Id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting role: {Id}", id);

        var roleDto = await roleQueries.GetByIdAsync(id, cancellationToken);
        if (roleDto is null)
        {
            logger.LogWarning("Role not found: {Id}", id);
            throw new Exception("Role not found");
        }

        var role = mapper.Map<Role>(roleDto);
        await unitOfWork.Roles.DeleteAsync(role, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Role deleted successfully: {Id}", id);
    }

    public async Task AssignPermissionsAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Assigning permissions to role: {RoleId}", roleId);

        var roleDto = await roleQueries.GetByIdAsync(roleId, cancellationToken);
        if (roleDto is null)
        {
            logger.LogWarning("Role not found: {RoleId}", roleId);
            throw new Exception("Role not found");
        }

        mapper.Map<Role>(roleDto);

        if (permissionIds == null || !permissionIds.Any())
        {
            logger.LogWarning("No permissions provided for role: {RoleId}", roleId);
            throw new Exception("No permissions provided");
        }

        foreach (var permissionId in permissionIds)
        {
            var permission = new RolePermission(roleId, permissionId);  
            await unitOfWork.RolePermissions.AddAsync(permission, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Permissions assigned successfully to role: {RoleId}", roleId);
    }
    
    Task<RoleDto?> IRoleService.GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => roleQueries.GetByIdAsync(id, cancellationToken);
}