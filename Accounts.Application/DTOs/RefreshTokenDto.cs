namespace Accounts.Application.DTOs;

public class RefreshTokenDto
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public string Token { get; init; } = string.Empty;
    
    public DateTime ExpiresAt { get; init; }
    
    public bool IsActive { get; init; }
}