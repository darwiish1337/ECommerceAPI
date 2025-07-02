using Accounts.Domain.Users;

namespace Accounts.Application.Interfaces.Persistence.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task DeleteAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string token, string revokedByIp, CancellationToken cancellationToken = default);
    Task RevokeAllTokensForUserAsync(Guid userId, string ipAddress, CancellationToken cancellationToken = default);
}