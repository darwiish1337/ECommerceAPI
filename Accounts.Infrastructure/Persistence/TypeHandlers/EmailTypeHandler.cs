using System.Data;
using Accounts.Domain.ValueObjects;
using Dapper;

namespace Accounts.Infrastructure.Persistence.TypeHandlers;

public class EmailTypeHandler : SqlMapper.TypeHandler<Email>
{
    public override void SetValue(IDbDataParameter parameter, Email? value)
    {
        parameter.Value = value?.Value ?? (object)DBNull.Value;
        if (value != null) Console.WriteLine("Parsing Email: " + value);
    }

    public override Email Parse(object? value)
    {
        if (value == null || value == DBNull.Value || string.IsNullOrEmpty(value.ToString()))
            return null!;
        Console.WriteLine("Parsing Email: " + value);

        // استخدم FromTrustedSource لأن البيانات جايه من قاعدة البيانات
        return Email.FromTrustedSource(value.ToString()!);
    }
}