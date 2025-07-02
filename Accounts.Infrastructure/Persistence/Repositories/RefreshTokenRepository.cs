using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(IShardDbContextFactory contextFactory) : IRefreshTokenRepository
{
    private readonly AccountsDbContext _context = contextFactory.CreateDbContext();
    
    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(token, cancellationToken);
    }

    public Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Remove(token);
        return Task.CompletedTask;
    }
    
    public async Task RevokeTokenAsync(string token, string revokedByIp, CancellationToken cancellationToken = default)
    {
        var entity = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken)
                     ?? throw new InvalidOperationException("Refresh token not found");

        entity.Revoke(revokedByIp);
    }
    
    public async Task RevokeAllTokensForUserAsync(Guid userId, string ipAddress, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke(ipAddress, "manual-logout");
        }
    }

}