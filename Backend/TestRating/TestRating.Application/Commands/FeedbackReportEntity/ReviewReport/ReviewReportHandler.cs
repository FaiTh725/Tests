using Application.Shared.Exceptions;
using MediatR;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.FeedbackReportEntity.ReviewReport
{
    public class ReviewReportHandler : 
        IRequestHandler<ReviewReportCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public ReviewReportHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            ReviewReportCommand request, 
            CancellationToken cancellationToken)
        {
            var report = await unitOfWork.ReportRepository
                .GetFeedbackReport(request.ReportId, cancellationToken);

            if(report is null)
            {
                throw new BadRequestException("Feedback report doesnt exist");
            }

            report.ReviewReport(request.IsApproved);

            await unitOfWork.ReportRepository
                .UpdateFeedbackReport(request.ReportId, report, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
