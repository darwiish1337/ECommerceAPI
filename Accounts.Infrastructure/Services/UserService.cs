using Accounts.Application.DTOs.Requests.User;
using Accounts.Application.Interfaces.Persistence;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Application.Interfaces.Persistence.Repositories;
using Accounts.Application.Interfaces.Services;
using Accounts.Application.Mapping;
using Accounts.Domain.ValueObjects;
using Mapster;
using SharedKernel.Abstractions.Auth;
using SharedKernel.Helpers;
using SharedKernel.Results;

namespace Accounts.Infrastructure.Services;

public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IUserQueries userQueries,
    IRoleQueries roleQueries, ICurrentRequestContext requestContext, IEmailService emailService) : IUserService
{
    public async Task<Result> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var dto = await userQueries.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return Result.Failure("User not found");

        var user = dto.ToDomain();
        user.Update(request.Username, request.FullName);

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var dto = await userQueries.GetByIdAsync(id, cancellationToken);
        if (dto is null)
            return Result.Failure("User not found");

        var user = dto.ToDomain();
        await userRepository.DeleteAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> AssignRolesAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken)
    {
        var userDto = await userQueries.GetByIdWithRolesAsync(userId, cancellationToken);
        if (userDto is null)
            return Result.Failure("User not found");

        var user = userDto.ToDomain();
        var roleDtos = await roleQueries.GetByIdsAsync(roleIds, cancellationToken);
        if (!roleDtos.Any())
            return Result.Failure("No valid roles provided");
        var roles = roleDtos.Adapt<List<Domain.Users.Role>>();

        user.AssignRoles(roles);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> ChangePasswordAsync(string oldPassword, string newPassword, CancellationToken cancellationToken)
    {
        var userId = requestContext.UserId;
        if (userId == null) return Result.Failure("User not authenticated");

        // Get userDto from the query
        var userDto = await userQueries.GetByIdAsync(userId.Value, cancellationToken);
        if (userDto == null) return Result.Failure("User not found");

        // Convert UserDto to User entity using mapping
        var user = userDto.ToDomain(); // Assuming you've defined the ToDomain method for UserDto

        // Check if the old password is correct
        if (!user.PasswordHash.Verify(oldPassword)) 
            return Result.Failure("Old password is incorrect");

        user.PasswordHash = PasswordHash.FromPlainText(newPassword);

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Password changed successfully");
    }
    public async Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken)
    {
        var user = await userQueries.GetByEmailAsync(email, cancellationToken);
        if (user == null) return Result.Failure("User not found");

        var verificationCode = VerificationCodeGenerator.Generate();
        await emailService.SendVerificationCodeAsync(email, verificationCode, cancellationToken);

        return Result.Success("Password reset code sent to email");
    }
    public async Task<Result> ChangeEmailAsync(string newEmail, string password, CancellationToken cancellationToken)
    {
        var userId = requestContext.UserId;
        if (userId == null) return Result.Failure("User not authenticated");

        // Get userDto from the query
        var userDto = await userQueries.GetByIdAsync(userId.Value, cancellationToken);
        if (userDto == null) return Result.Failure("User not found");

        // Convert UserDto to User entity using mapping
        var user = userDto.ToDomain(); // Assuming you've defined the ToDomain method for UserDto

        // Check if the password is correct
        if (!user.PasswordHash.Verify(password)) 
            return Result.Failure("Incorrect password");

        var verificationCode = VerificationCodeGenerator.Generate();
        await emailService.SendVerificationCodeAsync(newEmail, verificationCode, cancellationToken);

        user.Email = Email.FromTrustedSource(newEmail);
        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Email change requested. Please verify your new email address.");
    }
}

