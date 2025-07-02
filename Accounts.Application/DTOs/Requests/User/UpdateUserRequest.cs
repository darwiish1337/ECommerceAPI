namespace Accounts.Application.DTOs.Requests.User;

public class UpdateUserRequest
{
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
}
