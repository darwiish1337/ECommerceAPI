namespace Accounts.Application.DTOs.Requests.Auth;

public record ResendCodeRequest(Guid UserId, string Email);
