using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Domain.Validators;
using FluentValidation;

namespace Authorization.API.Validators.UserEntity
{
    public class LoginUserValidator : AbstractValidator<LoginCommand>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(UserValidator.IsValidEmail)
                    .WithMessage("Invalid email signature");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(UserValidator.MIN_PASSWORD_LENGTH)
                    .WithMessage("Min password length is " +
                    UserValidator.MIN_PASSWORD_LENGTH.ToString())
                .MaximumLength(UserValidator.MAX_PASSWORD_LENGTH)
                    .WithMessage("Max password length is " +
                    UserValidator.MAX_PASSWORD_LENGTH.ToString())
                .Must(UserValidator.IsValidPassword)
                    .WithMessage("Password must has one letter and one number");
        }
    }
}
