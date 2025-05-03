using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Queries.FeedbackReportEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.FeedbackReportEntity.SendReport
{
    public class SendReportHandler :
        IRequestHandler<SendReportCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;

        public SendReportHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(
            SendReportCommand request, 
            CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackById(request.ReportedFeedbackId, cancellationToken);

            if (feedback is null)
            {
                throw new BadRequestException("Feedback doesnt exist");
            }

            var profile = await unitOfWork.ProfileRepository
                .GetProfileById(request.ReviewerId, cancellationToken);

            if (profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var existedReport = await unitOfWork.ReportRepository
                .GetFeedbackReportByCriteria(
                new ReportProfileToFeedbackSpecification(
                    request.ReviewerId, 
                    request.ReportedFeedbackId), 
                cancellationToken);
        
            if(existedReport is not null)
            {
                throw new ConflictException("Current profile has already sent a report to current feedback");
            }

            var report = FeedbackReport.Initialize(
                request.Message,
                request.ReportedFeedbackId,
                request.ReviewerId);

            if(report.IsFailure)
            {
                throw new BadRequestException("Invalid request value - " +
                    report.Error);
            }

            var reportDb = await unitOfWork.ReportRepository
                .AddReport(report.Value, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return reportDb.Id;
        }
    }
}
