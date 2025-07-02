using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Domain.Enums;
using Accounts.Domain.Users;

namespace Accounts.Infrastructure.Persistence.Repositories;

public class UserVerificationRepository(IShardDbContextFactory contextFactory) : IUserVerificationRepository
{
    private readonly AccountsDbContext _context = contextFactory.CreateDbContext();
    
    public async Task AddAsync(UserVerification verification, CancellationToken cancellationToken)
    {
        await _context.UserVerifications.AddAsync(verification, cancellationToken);
    }

    public void MarkAsUsed(UserVerification verification)
    {
        verification.MarkAsUsed();
    }

    public void Update(UserVerification verification)
    {
        _context.UserVerifications.Update(verification);
    }
}
