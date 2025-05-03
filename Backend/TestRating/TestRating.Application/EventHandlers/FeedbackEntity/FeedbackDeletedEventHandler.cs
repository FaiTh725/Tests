using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Email;
using TestRating.Application.Queries.FeedbackEntity.Specifications;
using TestRating.Domain.Events;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.EventHandlers.FeedbackEntity
{
    public class FeedbackDeletedEventHandler :
        INotificationHandler<FeedbackDeletedEvent>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBus bus;
        private readonly ILogger<FeedbackDeletedEventHandler> logger;

        public FeedbackDeletedEventHandler(
            IUnitOfWork unitOfWork,
            IBus bus,
            ILogger<FeedbackDeletedEventHandler> logger)
        {
            this.unitOfWork = unitOfWork;
            this.bus = bus;
            this.logger = logger;
        }

        public async Task Handle(
            FeedbackDeletedEvent notification, 
            CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackExcludeFiltersById(notification.FeedbackId, cancellationToken);

            if( feedback is null)
            {
                logger.LogError($"Feedback with id = {notification.FeedbackId} doesnt exist. " +
                    $"Feedback Deleted Event Handler doesnt processing");
                return;
            }

            var feedbackOwner = await unitOfWork.ProfileRepository
                .GetProfileById(feedback.OwnerId, cancellationToken);

            if(feedbackOwner is null)
            {
                logger.LogError($"Feedback owner doesnt not exist");
                return;
            }

            await bus.Publish(new SendEmailRequest
            {
                Consumer = feedbackOwner.Email,
                Subject = "Feedback is deleted",
                Message = "Administrator decided to delete your feedback, " +
                "because it violate the rules"
            }, cancellationToken);
        }
    }
}
