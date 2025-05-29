using FluentValidation;
using Test.API.Contracts.Test;
using Test.Domain.Enums;
using Test.Domain.Validators;

namespace Test.API.Validators.TestValidators
{
    public class CreateTestValidator: AbstractValidator<CreateTestRequest>
    {
        public CreateTestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(TestValidator.MIN_NAME_LENGTH)
                    .WithMessage("Min test name length is " +
                    TestValidator.MIN_NAME_LENGTH.ToString())
                .MaximumLength(TestValidator.MAX_NAME_LENGTH)
                    .WithMessage("Max test name length is " +
                    TestValidator.MAX_NAME_LENGTH.ToString());

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(TestValidator.MAX_DESCRIPTION_LENGTH)
                    .WithMessage("Max description length is " +
                    TestValidator.MAX_DESCRIPTION_LENGTH.ToString());

            RuleFor(x => x.TestType)
                .IsInEnum()
                    .WithMessage("Valid values for enum: [" +
                    $"{string.Join(" ", Enum.GetValues(typeof(TestType)))}]");
        }       
    }
}
