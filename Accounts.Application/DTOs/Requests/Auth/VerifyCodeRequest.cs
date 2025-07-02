namespace Accounts.Application.DTOs.Requests.Auth;

public record VerifyCodeRequest(Guid UserId, string Code);
