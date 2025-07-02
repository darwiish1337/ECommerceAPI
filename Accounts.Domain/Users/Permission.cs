using SharedKernel.Entities;

namespace Accounts.Domain.Users;

public class Permission : BaseEntity<Guid>, IAggregateRoot
{
    public string Name { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    protected Permission() { }

    public Permission(Guid id, string name) : base(id)
    {
        Name = name;
    }
    
    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Role name cannot be empty");

        Name = newName;
    }

}
