using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackReportEntity.Specifications
{
    public class ReportWithReviewerSpecification :
        Specification<FeedbackReport>
    {
        public ReportWithReviewerSpecification(
            long reportId)
        {
            AddCriteria(x => x.Id == reportId);
            AddInclude(x => x.Reviewer);
        }
    }
}
