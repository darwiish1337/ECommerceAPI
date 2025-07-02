using SharedKernel.DTOs;
using System.Security.Claims;

namespace SharedKernel.Helpers;

public static class ClaimsHelper
{
    public static List<Claim> ToClaims(JwtUserData user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
        };
        
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Permissions as custom claim (comma-separated)
        if (user.Permissions.Any() == true)
        {
            claims.Add(new Claim("permissions", string.Join(",", user.Permissions)));
        }

        return claims;
    }

    public static List<string> GetPermissions(ClaimsPrincipal user)
    {
        var permissionsClaim = user.FindFirst("permissions")?.Value;
        return string.IsNullOrWhiteSpace(permissionsClaim)
            ? new List<string>()
            : permissionsClaim.Split(',').ToList();
    }
}