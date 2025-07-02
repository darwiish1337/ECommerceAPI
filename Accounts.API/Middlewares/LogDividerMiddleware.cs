namespace Accounts.API.Middlewares;

public class LogDividerMiddleware(RequestDelegate next, ILogger<LogDividerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");
            
            logger.LogError("═══════════════════════════════════════════════════════════════════════════════");
            throw;
        }
    }
}