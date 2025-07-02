using SharedKernel.Entities;

namespace Accounts.Domain.Users;

public class RefreshToken : BaseEntity<Guid>, IAggregateRoot
{
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }

    public string CreatedByIp { get; private set; } = default!;
    public string? CreatedByUserAgent { get; private set; } 

    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? RevokedByUserAgent { get; private set; } 
    public string? ReplacedByToken { get; private set; }

    // Navigation
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    protected RefreshToken() { }

    public RefreshToken(
        string token,
        DateTime expiresAt,
        string createdByIp,
        Guid userId,
        string? createdByUserAgent = null)
    {
        Id = Guid.NewGuid();
        Token = token;
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
        CreatedByUserAgent = createdByUserAgent;
        UserId = userId;
    }

    public void Revoke(
        string revokedByIp,
        string? replacedByToken = null,
        string? revokedByUserAgent = null)
    {
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        RevokedByUserAgent = revokedByUserAgent;
        ReplacedByToken = replacedByToken;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsActive => RevokedAt == null && !IsExpired;
}