using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Domain.Users;

namespace Accounts.Infrastructure.Persistence.Repositories;

public class RoleRepository(AccountsDbContext context) : IRoleRepository
{
    public async Task AddAsync(Role role, CancellationToken cancellationToken = default) =>
        await context.Roles.AddAsync(role, cancellationToken);

    public async Task<bool> UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        context.Roles.Update(role);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Role role, CancellationToken cancellationToken = default)
    {
        context.Roles.Remove(role);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
