using Accounts.Application.DTOs;

namespace Accounts.Application.Interfaces.Persistence.Queries;

public interface IRefreshTokenQueries
{
    Task<RefreshTokenDto?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}