using Accounts.Domain.Enums;
using SharedKernel.Entities;

namespace Accounts.Domain.Users;

public sealed class UserVerification : BaseEntity<Guid>, IAggregateRoot
{
    public Guid UserId { get; private set; }

    public string Code { get; private set; } = default!;

    public DateTime ExpiresAt { get; private set; }

    public bool IsUsed { get; private set; }

    public VerificationType Type { get; private set; }

    // EF Constructor
    private UserVerification() { }

    public UserVerification(Guid userId, string code, DateTime expiresAt, VerificationType type)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Code = code;
        ExpiresAt = expiresAt;
        Type = type;
        IsUsed = false;
    }
    
    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;

    public void MarkAsUsed()
    {
        IsUsed = true;
        UpdatedAt = DateTime.UtcNow;
    }
}