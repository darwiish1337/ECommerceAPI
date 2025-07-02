using System.Data;
using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Application.Interfaces.Services;
using Accounts.Infrastructure.Configuration;
using Accounts.Infrastructure.Extensions;
using Accounts.Infrastructure.Persistence;
using Accounts.Infrastructure.Persistence.Queries;
using Accounts.Infrastructure.Persistence.Repositories;
using Accounts.Infrastructure.Services;
using Infrastructure.Common.Tenant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedKernel.Abstractions.Auth;
using SharedKernel.Abstractions.DateTime;
using SharedKernel.Implementations;

namespace Accounts.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Dapper
        DapperConfigurationExtensions.ConfigureDapper();

        // JWT configuration and service
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtService, JwtService>();

        // Shared utilities
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Tenant provider (extracts a tenant key from the HTTP header)
        services.AddScoped<ITenantProvider, HeaderTenantProvider>();

        // Tenant-based factory for EF Core DbContext
        services.AddScoped<IShardDbContextFactory, ShardDbContextFactory>();

        // Tenant-based factory for Dapper IDbConnection
        services.AddScoped<IShardDbConnectionFactory, ShardDbConnectionFactory>();

        // Inject tenant-specific DbContext
        services.AddScoped(provider =>
        {
            var tenantKey = provider.GetRequiredService<ITenantProvider>().GetTenantKey();
            var factory = provider.GetRequiredService<IShardDbContextFactory>();
            return factory.CreateDbContextForTenant(tenantKey);
        });

        // Inject tenant-specific IDbConnection (for Dapper)
        services.AddScoped<IDbConnection>(provider =>
        {
            var tenantKey = provider.GetRequiredService<ITenantProvider>().GetTenantKey();
            var factory = provider.GetRequiredService<IShardDbConnectionFactory>();
            return factory.CreateConnectionForTenant(tenantKey);
        });
        
        // Register Dapper Query Interfaces
        services.AddScoped<IUserQueries, UserQueries>();
        services.AddScoped<IRefreshTokenQueries, RefreshTokenQueries>();
        services.AddScoped<IPermissionQueries, PermissionQueries>();
        services.AddScoped<IRoleQueries, RoleQueries>();
        services.AddScoped<IUserVerificationQueries, UserVerificationQueries>();

        // Register Repositories (EF Core-based)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserVerificationRepository, UserVerificationRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        // Unit of a Work pattern (aggregates all repositories)
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Add Services
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserVerificationService, UserVerificationService>();
        
        return services;
    }

    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
    
}