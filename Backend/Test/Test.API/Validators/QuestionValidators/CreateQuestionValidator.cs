using FluentValidation;
using Test.API.Contracts.Question;
using Test.Domain.Enums;
using Test.Domain.Validators;

namespace Test.API.Validators.QuestionValidators
{
    public class CreateQuestionValidator: AbstractValidator<CreateQuestionRequest>
    {
        public CreateQuestionValidator()
        {
            RuleFor(x => x.TestQuestion)
                .NotEmpty()
                .MinimumLength(QuestionValidator.MIN_QUESTION_LENGTH)
                    .WithMessage("Min question length is " +
                    QuestionValidator.MIN_QUESTION_LENGTH.ToString())
                .MaximumLength(QuestionValidator.MAX_QUESTION_LENGTH)
                    .WithMessage("Max question length is " +
                    QuestionValidator.MAX_QUESTION_LENGTH.ToString());

            RuleFor(x => x.QuestionWeight)
                .Must(x => x >= QuestionValidator.MIN_QUESTION_WEIGHT)
                    .WithMessage("Wight should be greate than " +
                    QuestionValidator.MIN_QUESTION_WEIGHT.ToString());

            RuleFor(x => x.QuestionType)
                .IsInEnum()
                    .WithMessage("Valid values for enum: [" +
                    $"{string.Join(" ", Enum.GetValues(typeof(QuestionType)))}]");
        }
    }
}
