using Accounts.Domain.ValueObjects;

namespace Accounts.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public required Email Email { get; set; }
    
    public string FullName { get; set; } = string.Empty;
    
    public required PasswordHash PasswordHash { get; set; } 
    
    public List<string> Roles { get; set; } = new();
}