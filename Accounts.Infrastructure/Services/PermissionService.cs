using Accounts.Application.DTOs;
using Accounts.Application.DTOs.Requests.Permission;
using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Application.Interfaces.Services;
using Accounts.Domain.Users;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Accounts.Infrastructure.Services;

public class PermissionService(IPermissionRepository permissionRepository, IPermissionQueries permissionQueries, ILogger<PermissionService> logger,
    IMapper mapper) : IPermissionService
{
    public async Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all permissions via Dapper");
        return await permissionQueries.GetAllAsync(cancellationToken);
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting permission by id via Dapper: {Id}", id);
        return await permissionQueries.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Guid> CreateAsync(CreatePermissionRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new permission: {Name}", request.Name);

        var permission = new Permission(Guid.NewGuid(), request.Name);
        await permissionRepository.AddAsync(permission, cancellationToken);

        logger.LogInformation("Permission created with Id: {Id}", permission.Id);
        return permission.Id;
    }
    
    public async Task UpdateAsync(UpdatePermissionRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating permission with Id: {Id}", request.Id);

        var permissionDto = await permissionQueries.GetByIdAsync(request.Id, cancellationToken);
        if (permissionDto is null)
        {
            logger.LogWarning("Permission not found: {Id}", request.Id);
            throw new Exception("Permission not found");
        }
        
        var permission = mapper.Map<Permission>(permissionDto);
        
        permission.ChangeName(request.Name);
        await permissionRepository.UpdateAsync(permission, cancellationToken);

        logger.LogInformation("Permission updated successfully: {Id}", request.Id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting permission: {Id}", id);

        var permissionDto = await permissionQueries.GetByIdAsync(id, cancellationToken);
        if (permissionDto is null)
        {
            logger.LogWarning("Permission not found: {Id}", id);
            throw new Exception("Permission not found");
        }
        
        var permission = mapper.Map<Permission>(permissionDto);

        await permissionRepository.DeleteAsync(permission, cancellationToken);
        logger.LogInformation("Permission deleted successfully: {Id}", id);
    }
}