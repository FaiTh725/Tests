using Authorization.Application.Commands.Email.SendConfirmCode;
using Authorization.Domain.Validators;
using FluentValidation;

namespace Authorization.API.Validators.EmailCode
{
    public class SendEmailConfirmCodeValidator : AbstractValidator<SendConfirmCodeCommand>
    {
        public SendEmailConfirmCodeValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(UserValidator.IsValidEmail)
                    .WithMessage("Invalid email signature");
        }
    }
}
