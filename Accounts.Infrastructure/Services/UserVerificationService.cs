using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Application.Interfaces.Services;
using Accounts.Domain.Enums;
using Accounts.Domain.Users;
using Mapster;
using SharedKernel.Helpers;
using SharedKernel.Results;

namespace Accounts.Infrastructure.Services;

public class UserVerificationService(IUserVerificationRepository verificationRepo, IUserVerificationQueries verificationQueries,
    IEmailService emailService, IUnitOfWork unitOfWork ) : IUserVerificationService
{
    public async Task<Result> GenerateVerificationCodeAsync(Guid userId, VerificationType type, CancellationToken cancellationToken)
    {
        var existing = await verificationQueries
            .GetLatestUnexpiredAsync(userId, type, cancellationToken);

        if (existing is not null)
        {
            var entity = existing.Adapt<UserVerification>();
            verificationRepo.MarkAsUsed(entity);
        }

        var code = VerificationCodeGenerator.Generate();
        var expiresAt = DateTime.UtcNow.AddMinutes(10);

        var verification = new UserVerification(userId, code, expiresAt, type);

        await verificationRepo.AddAsync(verification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> SendVerificationCodeAsync(Guid userId, string email, VerificationType type, CancellationToken cancellationToken)
    {
        var latest = await verificationQueries.GetLatestUnexpiredAsync(userId, type, cancellationToken);

        if (latest is null)
            return Result.Failure("No verification code found to send");

        await emailService.SendVerificationCodeAsync(email, latest.Code, cancellationToken);
        return Result.Success();
    }
    public async Task<Result> ResendVerificationCodeAsync(Guid userId, string email, VerificationType type, CancellationToken cancellationToken)
    {
        var result = await GenerateVerificationCodeAsync(userId, type, cancellationToken);
        if (!result.IsSuccess) return result;

        return await SendVerificationCodeAsync(userId, email, type, cancellationToken);
    }
    public async Task<Result> VerifyCodeAsync(Guid userId, string code, VerificationType type, CancellationToken cancellationToken)
    {
        var dto = await verificationQueries.GetLatestUnexpiredAsync(userId, type, cancellationToken);

        if (dto is null)
            return Result.Failure("No verification code found");

        var verification = dto.Adapt<UserVerification>();

        if (verification.IsUsed)
            return Result.Failure("Verification code already used");

        if (verification.IsExpired())
            return Result.Failure("Verification code has expired");

        if (!verification.Code.Equals(code))
            return Result.Failure("Incorrect verification code");

        verification.MarkAsUsed();
        verificationRepo.Update(verification); // تأكد إنها موجودة في الريبوزيتوري
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Verification code confirmed");
    }
}
