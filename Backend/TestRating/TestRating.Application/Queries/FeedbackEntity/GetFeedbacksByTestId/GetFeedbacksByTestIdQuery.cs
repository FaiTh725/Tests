using MediatR;
using TestRating.Application.Contacts.Feedback;
using TestRating.Application.Contacts.Pagination;

namespace TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestId
{
    public class GetFeedbacksByTestIdQuery : 
        IRequest<BasePaginationResponse<FeedbackWithReviewsResponse>>
    {
        public long TestId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
