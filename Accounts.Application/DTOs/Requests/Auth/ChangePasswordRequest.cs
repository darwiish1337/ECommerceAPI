namespace Accounts.Application.DTOs.Requests.Auth;

public class ChangePasswordRequest
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}