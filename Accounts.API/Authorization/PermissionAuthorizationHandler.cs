using Microsoft.AspNetCore.Authorization;

namespace Accounts.API.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissionsClaim = context.User.FindFirst("permissions")?.Value;

        if (!string.IsNullOrWhiteSpace(permissionsClaim))
        {
            var permissions = permissionsClaim.Split(',');

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}