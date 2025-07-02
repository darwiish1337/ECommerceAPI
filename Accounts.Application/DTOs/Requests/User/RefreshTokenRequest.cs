namespace Accounts.Application.DTOs.Requests.User;

public class RefreshTokenRequest
{
    public string AccessToken { get; init; } = string.Empty;
    
    public string RefreshToken { get; init; } = string.Empty;
}