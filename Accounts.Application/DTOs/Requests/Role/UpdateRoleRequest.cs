namespace Accounts.Application.DTOs.Requests.Role;

public class UpdateRoleRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}