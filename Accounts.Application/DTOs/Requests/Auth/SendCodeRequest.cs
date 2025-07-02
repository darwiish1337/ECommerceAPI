using Accounts.Domain.Enums;

namespace Accounts.Application.DTOs.Requests.Auth;

public class SendCodeRequest
{
    public string Email { get; set; } = default!;
    public VerificationType Type { get; set; }
}
