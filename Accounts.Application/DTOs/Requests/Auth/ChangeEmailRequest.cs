namespace Accounts.Application.DTOs.Requests.Auth;

public class ChangeEmailRequest
{
    public string NewEmail { get; set; }
    public string Password { get; set; }
}