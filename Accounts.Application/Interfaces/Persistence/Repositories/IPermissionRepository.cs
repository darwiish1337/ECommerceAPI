using Accounts.Domain.Users;

namespace Accounts.Application.Interfaces.Persistence.Repositories;

public interface IPermissionRepository
{
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Permission permission, CancellationToken cancellationToken = default);
}
