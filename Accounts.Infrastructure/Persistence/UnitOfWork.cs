using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Repositories;

namespace Accounts.Infrastructure.Persistence;

public class UnitOfWork(AccountsDbContext context, IUserRepository users, IRoleRepository roles,
    IPermissionRepository permissions, IRefreshTokenRepository refreshTokens ) : IUnitOfWork
{
    public IUserRepository Users => users;
    public IRoleRepository Roles => roles;
    public IPermissionRepository Permissions => permissions;
    public IRefreshTokenRepository RefreshTokens => refreshTokens;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
