using Accounts.Domain.ValueObjects;
using SharedKernel.Entities;

namespace Accounts.Domain.Users;

public class User : BaseEntity<Guid>, IAggregateRoot
{
    public string Username { get; private set; } = string.Empty;
    public Email Email { get; set; }
    public PasswordHash PasswordHash { get; set; }
    public string FullName { get; private set; } = string.Empty;
    public ICollection<Role> Roles { get; private set; } = new List<Role>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public bool IsEmailConfirmed { get; private set; }

    
    protected User() { }

    public User(Guid id, string username, Email email, PasswordHash passwordHash, string fullName)
        : base(id)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
    }
    
    public void ChangePassword(string newPassword)
    {
        PasswordHash = PasswordHash.FromPlainText(newPassword);
        
        // Domain Event
        // AddDomainEvent(new PasswordChangedEvent(Id));
    }
    
    public void Update(string username, string fullName)
    {
        Username = username;
        FullName = fullName;
    }
    
    public void ConfirmEmail() => IsEmailConfirmed = true;
    
    public void UpdateName(string newFullName)
    {
        FullName = newFullName;
    }
    
    public bool VerifyPassword(string password)
        => PasswordHash.Verify(password);
    
    public void ChangeEmail(Email newEmail)
    {
        if (newEmail == null)
            throw new ArgumentNullException(nameof(newEmail));

        // Business rule: لا يمكن تغيير الإيميل لنفس القيمة
        if (Email.Value.Equals(newEmail.Value, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("New email must be different from current email");

        Email = newEmail;
        
        // يمكن إضافة Domain Event هنا
        // AddDomainEvent(new EmailChangedEvent(Id, Email, newEmail));
    }
    
    public bool HasCompanyEmail()
    {
        return Email.IsCompanyEmail();
    }

    public bool IsFromDomain(string domain)
    {
        return Email.IsFromDomain(domain);
    }
    
    public void AssignRoles(List<Role> roles)
    {
        Roles.Clear();
        foreach (var role in roles)
        {
            Roles.Add(role);
        }
    }
    
}
