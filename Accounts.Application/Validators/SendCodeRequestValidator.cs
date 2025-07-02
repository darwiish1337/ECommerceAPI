using Accounts.Application.DTOs.Requests;
using Accounts.Application.DTOs.Requests.Auth;
using FluentValidation;

namespace Accounts.Application.Validators;

public class SendCodeRequestValidator : AbstractValidator<SendCodeRequest>
{
    public SendCodeRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Type).IsInEnum();
    }
}
