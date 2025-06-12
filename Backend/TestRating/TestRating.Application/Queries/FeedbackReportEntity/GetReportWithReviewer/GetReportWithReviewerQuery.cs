using MediatR;
using TestRating.Application.Contacts.FeedbackReport;

namespace TestRating.Application.Queries.FeedbackReportEntity.GetReportWithReviewer
{
    public class GetReportWithReviewerQuery :
        IRequest<FeedbackReportWithReviewer>
    {
        public long Id { get; set; }
    }
}
