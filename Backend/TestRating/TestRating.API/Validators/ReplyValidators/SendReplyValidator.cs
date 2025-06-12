using FluentValidation;
using TestRating.API.Contracts.FeedbackReply;
using TestRating.Domain.Validators;

namespace TestRating.API.Validators.ReplyValidators
{
    public class SendReplyValidator :
        AbstractValidator<SendReplyRequest>
    {
        public SendReplyValidator()
        {
            RuleFor(x => x.Text)
                .MinimumLength(FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH)
                    .WithMessage("Min message length is " +
                    FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH.ToString())
                .MaximumLength(FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH)
                    .WithMessage("Max message length is " +
                    FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH.ToString());
        }
    }
}
