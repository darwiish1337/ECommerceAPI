using Accounts.Domain.Users;

namespace Accounts.Application.Interfaces.Persistence.Repositories;

public interface IRoleRepository
{
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Role role, CancellationToken cancellationToken = default);
}
