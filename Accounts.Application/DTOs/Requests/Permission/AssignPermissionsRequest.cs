namespace Accounts.Application.DTOs.Requests.Permission;

public class AssignPermissionsRequest
{
    public Guid RoleId { get; set; }
    public List<Guid> PermissionIds { get; set; } = [];
}