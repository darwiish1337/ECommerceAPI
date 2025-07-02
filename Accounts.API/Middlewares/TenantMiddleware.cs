using Infrastructure.Common.Tenant;

namespace Accounts.API.Middlewares;

public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        if (!context.Request.Headers.TryGetValue("X-Tenant", out var tenantKey))
        {
            Console.WriteLine("❌ X-Tenant header is missing.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing X-Tenant header.");
            return;
        }

        Console.WriteLine($"✅ X-Tenant header found: {tenantKey}");

        tenantProvider.SetTenant(tenantKey!);
        await next(context);
    }
}