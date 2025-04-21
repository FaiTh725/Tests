using FluentValidation;
using Test.API.Contracts.Test;
using Test.Domain.Validators;

namespace Test.API.Validators.TestValidators
{
    public class UpdateTestValidator: AbstractValidator<UpdateTestRequest>
    {
        public UpdateTestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(TestValidator.MIN_NAME_LENGHT)
                    .WithMessage("Min test name length is " +
                    TestValidator.MIN_NAME_LENGHT.ToString())
                .MaximumLength(TestValidator.MAX_NAME_LENGHT)
                    .WithMessage("Max test name length is " +
                    TestValidator.MAX_NAME_LENGHT.ToString());

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(TestValidator.MAX_DESCRIPTION_LENGHT)
                    .WithMessage("Max description length is " +
                    TestValidator.MAX_DESCRIPTION_LENGHT.ToString());
        }
    }
}
