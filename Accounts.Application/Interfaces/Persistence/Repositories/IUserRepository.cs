using Accounts.Domain.Users;

namespace Accounts.Application.Interfaces.Persistence.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
}
