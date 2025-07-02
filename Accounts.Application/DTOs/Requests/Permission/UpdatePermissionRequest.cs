namespace Accounts.Application.DTOs.Requests.Permission;

public class UpdatePermissionRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}