using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackReportEntity.Specifications
{
    public class ReportProfileToFeedbackSpecification : 
        Specification<FeedbackReport>
    {
        public ReportProfileToFeedbackSpecification(
            long profileId,
            long feedbackId)
        {
            AddCriteria(report => 
                report.ReviewerId == profileId &&
                report.ReportedFeedbackId == feedbackId);
        }
    }
}
