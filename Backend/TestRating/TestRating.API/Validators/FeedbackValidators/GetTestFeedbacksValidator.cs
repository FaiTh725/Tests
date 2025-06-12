using FluentValidation;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestId;

namespace TestRating.API.Validators.FeedbackValidators
{
    public class GetTestFeedbacksValidator : 
        AbstractValidator<GetFeedbacksByTestIdQuery>
    {
        public GetTestFeedbacksValidator()
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
