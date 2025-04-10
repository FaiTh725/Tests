using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Domain.Validators;
using FluentValidation;

namespace Authorization.API.Validators.UserEntity
{
    public class RegisterUserValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(UserValidator.IsValidEmail)
                    .WithMessage("Invalid email signature");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(UserValidator.MIN_PASSWORD_LENGHT)
                    .WithMessage("Min password length is " +
                    UserValidator.MIN_PASSWORD_LENGHT.ToString())
                .MaximumLength(UserValidator.MAX_PASSWORD_LENGHT)
                    .WithMessage("Max password length is " +
                    UserValidator.MAX_PASSWORD_LENGHT.ToString())
                .Must(UserValidator.IsValidPassword)
                    .WithMessage("Password must has one letter and one number");

            RuleFor(x => x.UserName)
                .NotEmpty();
        }
    }
}
