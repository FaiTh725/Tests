using MediatR;

namespace TestRating.Application.Commands.FeedbackReportEntity.ReviewReport
{
    public class ReviewReportCommand : IRequest
    {
        public long ReportId { get; set; }

        public bool IsApproved { get; set; }
    }
}
