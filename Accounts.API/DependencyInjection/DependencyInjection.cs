using Accounts.API.Authorization;
using Accounts.API.Services;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using SharedKernel.Abstractions.Auth;

namespace Accounts.API.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentRequestContext, CurrentRequestContext>();

        
        return services;
    }
    
    public static void AddLogging(this WebApplicationBuilder builder)
    {
        // Clear default providers
        builder.Logging.ClearProviders();
        
        builder.Logging.AddSimpleConsole(options =>
        {
            options.SingleLine = false;
            options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
            options.IncludeScopes = true;
        });
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.Console()
            .WriteTo.File("Logs/errors.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}
