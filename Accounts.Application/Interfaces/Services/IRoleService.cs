using Accounts.Application.DTOs;
using Accounts.Application.DTOs.Requests.Role;
using CreateRoleRequest = Accounts.Application.DTOs.Requests.Role.CreateRoleRequest;

namespace Accounts.Application.Interfaces.Services;

public interface IRoleService
{
    Task<Guid> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task AssignPermissionsAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default);
    Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}