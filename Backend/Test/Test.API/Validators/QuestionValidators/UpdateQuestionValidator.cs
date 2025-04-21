using FluentValidation;
using Test.API.Contracts.Question;
using Test.Domain.Validators;

namespace Test.API.Validators.QuestionValidators
{
    public class UpdateQuestionValidator: AbstractValidator<UpdateQuestionRequest>
    {
        public UpdateQuestionValidator()
        {
            RuleFor(x => x.TestQuestion)
                .NotEmpty()
                .MinimumLength(QuestionValidator.MIN_QUESTION_LENGHT)
                    .WithMessage("Min question length is " +
                    QuestionValidator.MIN_QUESTION_LENGHT.ToString())
                .MaximumLength(QuestionValidator.MAX_QUESTION_LENGHT)
                    .WithMessage("Max question length is " +
                    QuestionValidator.MAX_QUESTION_LENGHT.ToString());

            RuleFor(x => x.QuestionWeight)
                .Must(x => x >= QuestionValidator.MIN_QUESTION_WEIGHT)
                    .WithMessage("Wight should be greate than " +
                    QuestionValidator.MIN_QUESTION_WEIGHT.ToString());
        }
    }
}
