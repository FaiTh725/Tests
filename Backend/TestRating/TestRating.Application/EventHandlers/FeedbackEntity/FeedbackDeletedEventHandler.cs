using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Email;
using TestRating.Application.Contacts.Feedback;
using TestRating.Domain.Events;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.EventHandlers.FeedbackEntity
{
    public class FeedbackDeletedEventHandler :
        INotificationHandler<FeedbackDeletedEvent>
    {
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IUnitOfWork unitOfWork;

        public FeedbackDeletedEventHandler(
            IPublishEndpoint publishEndpoint, 
            IUnitOfWork unitOfWork)
        {
            this.publishEndpoint = publishEndpoint;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            FeedbackDeletedEvent notification, 
            CancellationToken cancellationToken)
        {
            await publishEndpoint.Publish(new FeedbackDeletedRequest
            {
                FeedbackId = notification.FeedbackId
            }, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
