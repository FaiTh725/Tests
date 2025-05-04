using FluentValidation;
using TestRating.API.Contracts.Feedback;
using TestRating.Domain.Validators;

namespace TestRating.API.Validators.FeedbackValidators
{
    public class CreateFeedbackValidator : 
        AbstractValidator<CreateFeedbackRequest>
    {
        public CreateFeedbackValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .MinimumLength(FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH)
                    .WithMessage("Minimum message length - " + FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH.ToString())
                .MaximumLength(FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH)
                    .WithMessage("Maximum message length - " + FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH.ToString());

            RuleFor(x => x.Rating)
                .Must(x => x >= FeedbackValidator.MIN_FEEDBACK_RATING &&
                    x <= FeedbackValidator.MAX_FEEDBACK_RATING)
                    .WithMessage("Rating should be in range " +
                    $"[{FeedbackValidator.MIN_FEEDBACK_RATING}, {FeedbackValidator.MAX_FEEDBACK_RATING}]");
        }
    }
}
