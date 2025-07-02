using SharedKernel.Entities;

namespace Accounts.Domain.Users;

public class RolePermission : BaseEntity<Guid>, IAggregateRoot
{
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = default!;

    public Guid PermissionId { get; private set; }
    public Permission Permission { get; private set; } = default!;

    protected RolePermission() { }

    public RolePermission(Guid roleId, Guid permissionId)
    {
        Id = Guid.NewGuid();
        RoleId = roleId;
        PermissionId = permissionId;
    }
}