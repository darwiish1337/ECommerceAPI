using Accounts.Application.DTOs;
using Accounts.Domain.Enums;

namespace Accounts.Application.Interfaces.Persistence.Queries;

public interface IUserVerificationQueries
{
    Task<UserVerificationDto?> GetLatestUnexpiredAsync(Guid userId, VerificationType type, CancellationToken cancellationToken);
}
