using MediatR;
using Microsoft.Extensions.Logging;
using TestRating.Application.Commands.FeedbackEntity.BanFeedback;
using TestRating.Domain.Events;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.EventHandlers.FeedbackReportEntity
{
    public class ReportReviewedEventHandler :
        INotificationHandler<FeedbackReportReviewedEvent>
    {
        private readonly IMediator mediator;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<ReportReviewedEventHandler> logger;

        public ReportReviewedEventHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            ILogger<ReportReviewedEventHandler> logger)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task Handle(
            FeedbackReportReviewedEvent notification, 
            CancellationToken cancellationToken)
        {
            var report = await unitOfWork.ReportRepository
                .GetFeedbackReport(notification.FeedbackReportId, cancellationToken);

            if(report is null)
            {
                logger.LogError($"Feedback Report with id = {notification.FeedbackReportId}. " +
                    "Reviewed Report Event Handler cant process invalid feedback report ");
                return;
            }

            await mediator.Send(new BanFeedbackCommand
            {
                Id = report.ReportedFeedbackId,
            }, cancellationToken);
        }
    }
}
