using System.Security.Claims;
using SharedKernel.Abstractions.Auth;

namespace Accounts.API.Services;

public class CurrentRequestContext(IHttpContextAccessor accessor) : ICurrentRequestContext
{
    public string IpAddress =>
        accessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    public string? UserAgent =>
        accessor.HttpContext?.Request.Headers["User-Agent"].ToString();

    public Guid? UserId
    {
        get
        {
            var userIdClaim = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim?.Value, out var id) ? id : null;
        }
    }

    public string? TenantId =>
        accessor.HttpContext?.Request.Headers["X-Tenant-Id"].ToString();
}