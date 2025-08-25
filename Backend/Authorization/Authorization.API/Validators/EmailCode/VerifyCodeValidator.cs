using Authorization.Application.Commands.Email.VerifyCode;
using Authorization.Domain.Validators;
using FluentValidation;
using System.Security.Cryptography.Xml;

namespace Authorization.API.Validators.EmailCode
{
    public class VerifyCodeValidator : AbstractValidator<VerifyCodeCommand>
    {
        public VerifyCodeValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(UserValidator.IsValidEmail)
                    .WithMessage("Invalid email signature");
        }
    }
}
