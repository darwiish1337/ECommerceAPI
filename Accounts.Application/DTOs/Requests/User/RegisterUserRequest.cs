using Accounts.Domain.ValueObjects;

namespace Accounts.Application.DTOs.Requests.User;

public class RegisterUserRequest
{
    public string Username { get; init; } = string.Empty;
    
    public Email Email { get; init; } = null!;
    
    public string Password { get; init; } = string.Empty;
    
    public string FullName { get; init; } = string.Empty;
}