using Accounts.Domain.Users;

namespace Accounts.Application.Interfaces.Persistence.Repositories;

public interface IUserVerificationRepository
{
    Task AddAsync(UserVerification verification, CancellationToken cancellationToken);
    void MarkAsUsed(UserVerification verification);
    void Update(UserVerification verification);
}
