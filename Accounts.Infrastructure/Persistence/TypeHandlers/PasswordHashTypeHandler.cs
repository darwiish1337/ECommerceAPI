using System.Data;
using Accounts.Domain.ValueObjects;
using Dapper;

namespace Accounts.Infrastructure.Persistence.TypeHandlers;

public class PasswordHashTypeHandler : SqlMapper.TypeHandler<PasswordHash>
{
    public override void SetValue(IDbDataParameter parameter, PasswordHash? value)
    {
        parameter.Value = value?.Value ?? (object)DBNull.Value;
        if (value != null) Console.WriteLine("Parsing Email: " + value);
    }

    public override PasswordHash Parse(object? value)
    {
        if (value == null || value == DBNull.Value || string.IsNullOrEmpty(value.ToString()))
            return null!;
        Console.WriteLine("Parsing Email: " + value);
        return PasswordHash.FromHashed(value.ToString()!);
    }
}