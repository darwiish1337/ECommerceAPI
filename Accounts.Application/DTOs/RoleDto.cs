namespace Accounts.Application.DTOs;

public class RoleDto
{
    public Guid Id { get; init; }
    
    public string Name { get; init; } = string.Empty;
    
    public List<string> Permissions { get; set; } = new();
}