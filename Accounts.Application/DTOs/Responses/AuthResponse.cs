namespace Accounts.Application.DTOs.Responses;

public class AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public bool IsEmailConfirmed { get; init; }
}