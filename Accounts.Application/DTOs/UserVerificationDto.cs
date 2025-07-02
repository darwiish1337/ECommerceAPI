using Accounts.Domain.Enums;

namespace Accounts.Application.DTOs;

public class UserVerificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Code { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public VerificationType Type { get; set; }
}
