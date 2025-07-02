using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Domain.Users;

namespace Accounts.Infrastructure.Persistence.Repositories;

public class PermissionRepository(IShardDbContextFactory contextFactory) : IPermissionRepository
{
    private readonly AccountsDbContext _context = contextFactory.CreateDbContext();

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default) =>
        await _context.Permissions.AddAsync(permission, cancellationToken);

    public async Task<bool> UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Update(permission);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Remove(permission);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}