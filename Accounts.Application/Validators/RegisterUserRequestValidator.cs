using Accounts.Application.DTOs.Requests.User;
using Accounts.Application.Interfaces.Persistence.Queries;
using Accounts.Domain.ValueObjects;
using FluentValidation;
using SharedKernel.Exceptions;

namespace Accounts.Application.Validators;

/// <summary>
/// Validator for user registration requests
/// </summary>
public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    private const int MinPasswordLength = 6;
    private const string EmailRequired = "Email is required";
    private const string InvalidEmail = "Invalid email";
    private const string EmailExists = "Email already exists";
    private const string UsernameRequired = "Username is required";
    private const string UsernameExists = "Username already exists";

    private readonly IUserQueries _userQueries;

    public RegisterUserRequestValidator(IUserQueries userQueries)
    {
        _userQueries = userQueries ?? throw new DomainException(nameof(userQueries));

        ConfigureEmailRules();
        ConfigureUsernameRules();
        ConfigurePasswordRules();
    }

    private void ConfigureEmailRules()
    {
        RuleFor(x => x.Email)
            .NotNull().WithMessage(EmailRequired)
            .Must(email => !string.IsNullOrWhiteSpace(email?.Value))
            .WithMessage(EmailRequired)
            .Must(email => email?.Value.Length <= 256)
            .WithMessage("Email is too long")
            .MustAsync(async (email, ct) =>
            {
                if (email == null) return false;
                return await _userQueries.GetByEmailAsync(email, ct) is null;
            }).WithMessage(EmailExists);
    }

    private void ConfigureUsernameRules()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(UsernameRequired)
            .MustAsync(IsUsernameUnique).WithMessage(UsernameExists);
    }

    private void ConfigurePasswordRules()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(MinPasswordLength);
    }

    private async Task<bool> IsEmailUnique(string emailString, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(emailString))
            return false;

        var email = Email.Create(emailString); // أو new Email(emailString) حسب تصميمك
        return await _userQueries.GetByEmailAsync(email, token) is null;
    }

    private async Task<bool> IsUsernameUnique(string username, CancellationToken token)
        => await _userQueries.GetByUsernameAsync(username, token) is null;
}