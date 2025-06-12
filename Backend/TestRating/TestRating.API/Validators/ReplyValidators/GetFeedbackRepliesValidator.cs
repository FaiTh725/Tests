using FluentValidation;
using TestRating.Application.Queries.FeedbackReplyEntity.GetFeedbackReplies;

namespace TestRating.API.Validators.ReplyValidators
{
    public class GetFeedbackRepliesValidator :
        AbstractValidator<GetFeedbackRepliesQuery>
    {
        public GetFeedbackRepliesValidator()
        {
            RuleFor(x => x.Page)
                .Must(x => x > 0)
                    .WithMessage("Page less then one");

            RuleFor(x => x.PageSize)
                .Must(x => x > 0)
                    .WithMessage("Page size less then one");
        }
    }
}
