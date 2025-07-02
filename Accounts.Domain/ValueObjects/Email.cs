using System.Text.RegularExpressions;
using SharedKernel.ValueObjects;

namespace Accounts.Domain.ValueObjects;

public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public string Value { get; }

    private Email(string value)
    {
        Value = value.ToLowerInvariant(); // تحويل لـ lowercase
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        email = email.Trim();

        if (email.Length > 50) // RFC 5321 limit
            throw new ArgumentException("Email is too long", nameof(email));

        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        if (IsDisposableEmail(email))
            throw new ArgumentException("Disposable emails are not allowed", nameof(email));

        return new Email(email);
    }

    // إنشاء بدون validation (من قاعدة البيانات)
    public static Email FromTrustedSource(string email)
    {
        return new Email(email);
    }

    // Business Methods
    private string GetDomain()
    {
        var atIndex = Value.IndexOf('@');
        return atIndex > 0 ? Value.Substring(atIndex + 1) : string.Empty;
    }

    public string GetLocalPart()
    {
        var atIndex = Value.IndexOf('@');
        return atIndex > 0 ? Value.Substring(0, atIndex) : Value;
    }

    public bool IsFromDomain(string domain)
    {
        return GetDomain().Equals(domain, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsCompanyEmail()
    {
        var personalDomains = new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com" };
        return !personalDomains.Contains(GetDomain(), StringComparer.OrdinalIgnoreCase);
    }

    // Security & Business Rules
    private static bool IsDisposableEmail(string email)
    {
        var disposableDomains = new[]
        {
            "10minutemail.com", "tempmail.org", "guerrillamail.com", 
            "mailinator.com", "trashmail.com"
        };
        
        var domain = email.Substring(email.IndexOf('@') + 1);
        return disposableDomains.Contains(domain, StringComparer.OrdinalIgnoreCase);
    }

    // Implicit conversion للسهولة
    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new Email(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}