using Accounts.Application.DTOs.Requests.User;
using FluentValidation;

namespace Accounts.Application.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required.")
            .MinimumLength(20).WithMessage("Access token seems too short.")
            .Matches(@"^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+$")
            .WithMessage("Access token must be a valid JWT format.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MinimumLength(30).WithMessage("Refresh token is too short.");
    }
}