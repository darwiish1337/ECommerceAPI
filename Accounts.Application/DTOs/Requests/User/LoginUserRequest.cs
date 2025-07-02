using Accounts.Domain.ValueObjects;

namespace Accounts.Application.DTOs.Requests.User;

public class LoginUserRequest
{
    public Email Email { get; init; } = null!;
    
    public string Password { get; init; } = string.Empty;
}