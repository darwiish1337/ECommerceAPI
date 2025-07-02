using Microsoft.AspNetCore.Authorization;

namespace Accounts.API.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}