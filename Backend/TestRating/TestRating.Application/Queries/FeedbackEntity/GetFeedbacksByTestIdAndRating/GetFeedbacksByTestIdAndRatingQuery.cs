using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;
using TestRating.Application.Contacts.Feedback;
using TestRating.Application.Contacts.Pagination;

namespace TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestIdAndRating
{
    public class GetFeedbacksByTestIdAndRatingQuery :
         IRequest<BasePaginationResponse<FeedbackWithReviewsResponse>>,
        ICheckTestIsExist
    {
        public long TestId { get; set; }

        public int Rating {  get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
