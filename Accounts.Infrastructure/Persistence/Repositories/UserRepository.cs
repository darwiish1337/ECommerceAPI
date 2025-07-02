using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Domain.Users;

namespace Accounts.Infrastructure.Persistence.Repositories;

public class UserRepository(AccountsDbContext context) : IUserRepository
{
    private readonly AccountsDbContext _context = context;

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }
}