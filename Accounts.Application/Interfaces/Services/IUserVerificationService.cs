using Accounts.Domain.Enums;
using SharedKernel.Results;

namespace Accounts.Application.Interfaces.Services;

public interface IUserVerificationService
{
    Task<Result> VerifyCodeAsync(Guid userId, string code, VerificationType type, CancellationToken cancellationToken);
    Task<Result> ResendVerificationCodeAsync(Guid userId, string email, VerificationType type, CancellationToken cancellationToken);
    Task<Result> GenerateVerificationCodeAsync(Guid userId, VerificationType type, CancellationToken cancellationToken);
    Task<Result> SendVerificationCodeAsync(Guid userId, string email, VerificationType type, CancellationToken cancellationToken);
}
