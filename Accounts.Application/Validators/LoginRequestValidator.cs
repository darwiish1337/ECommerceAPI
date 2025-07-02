using Accounts.Application.DTOs.Requests.User;
using FluentValidation;

namespace Accounts.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotNull().WithMessage("Email is required");

        RuleFor(x => x.Email.Value)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}