using Accounts.Application.DTOs;

namespace Accounts.Application.Interfaces.Persistence.Queries;

public interface IPermissionQueries
{
    Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PermissionDto>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
}