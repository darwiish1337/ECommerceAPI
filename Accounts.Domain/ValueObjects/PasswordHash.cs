using SharedKernel.ValueObjects;

namespace Accounts.Domain.ValueObjects;

public class PasswordHash : ValueObject
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        Value = value;
    }

    public static PasswordHash FromPlainText(string password)
    {
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);
        return new PasswordHash(hashed);
    }

    public static PasswordHash FromHashed(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Password hash cannot be null or empty.", nameof(hash));

        return new PasswordHash(hash);
    }

    public bool Verify(string plainPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    // ✅ These two lines are the key to work with Dapper automatically
    public static implicit operator string(PasswordHash hash) => hash?.Value ?? "";
    
    public static implicit operator PasswordHash(string value)
        => string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(nameof(value))
            : FromHashed(value);
    
}
