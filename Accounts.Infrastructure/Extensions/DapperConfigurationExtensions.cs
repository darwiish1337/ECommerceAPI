using Accounts.Infrastructure.Persistence.TypeHandlers;
using Dapper;

namespace Accounts.Infrastructure.Extensions;

public static class DapperConfigurationExtensions
{
    public static void ConfigureDapper()
    {
        SqlMapper.AddTypeHandler(new PasswordHashTypeHandler());
        SqlMapper.AddTypeHandler(new EmailTypeHandler());
    }
}