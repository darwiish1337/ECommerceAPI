namespace Accounts.Application.DTOs.Responses;

public class UserDetailsResponse
{
    public Guid Id { get; init; }
    
    public string Username { get; init; } = string.Empty;
    
    public string Email { get; init; } = string.Empty;
    
    public string FullName { get; init; } = string.Empty;
    
    public List<string> Roles { get; init; } = new();
}