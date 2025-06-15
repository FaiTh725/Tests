using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;
using TestRating.Application.Contacts.Test;

namespace TestRating.Application.Queries.FeedbackEntity.GetTestStatisticsByFeedbacks
{
    public class GetTestStatisticsByFeedbacksQuery :
        IRequest<TestFeedbackStatistics>,
        ICheckTestIsExist
    {
        public long TestId { get; set; }
    }
}
