namespace Accounts.Application.DTOs.Requests.Role;

public class AssignRolesRequest
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = [];
}
