using Accounts.Application.DTOs;

namespace Accounts.Application.Interfaces.Persistence.Queries;

public interface IRoleQueries
{
    Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleWithPermissionsDto?> GetWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<RoleDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

}