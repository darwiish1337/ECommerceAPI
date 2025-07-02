using Accounts.Application.DTOs;
using Accounts.Application.DTOs.Requests;
using Accounts.Application.DTOs.Requests.Permission;

namespace Accounts.Application.Interfaces.Services;

public interface IPermissionService
{
    Task<Guid> CreateAsync(CreatePermissionRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdatePermissionRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}