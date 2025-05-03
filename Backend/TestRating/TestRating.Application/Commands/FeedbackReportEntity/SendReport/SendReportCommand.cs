using MediatR;

namespace TestRating.Application.Commands.FeedbackReportEntity.SendReport
{
    public class SendReportCommand : 
        IRequest<long>
    {
        public string Message { get; set; } = string.Empty;

        public long ReviewerId { get; set; }

        public long ReportedFeedbackId { get; set; }
    }
}
