using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Notification;
using TestRating.Application.Contacts.FeedbackReply;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbackWithOwner;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Consumers.ReplyConsumers
{
    public class NotifyReplyCreatedConsumer :
        IConsumer<ReplySentRequest>
    {
        private readonly ILogger<NotifyReplyCreatedConsumer> logger;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMediator mediator;

        public NotifyReplyCreatedConsumer(
            ILogger<NotifyReplyCreatedConsumer> logger,
            IPublishEndpoint publishEndpoint,
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
            this.unitOfWork = unitOfWork;
            this.mediator = mediator;
        }

        public async Task Consume(
            ConsumeContext<ReplySentRequest> context)
        {
            logger.LogInformation("Sent notification when reply created consumer get message");

            var reply = await unitOfWork.ReplyRepository
                .GetReplyByCriteria(new ReplyByIdWithOwnerSpecification(context.Message.ReplyId));
        
            if(reply is null)
            {
                logger.LogError("Reply doesnt exist");
                return;
            }

            var feedback = await mediator.Send(new GetFeedbackWithOwnerQuery
            {
                Id = reply.FeedbackId
            });

            if(feedback is null)
            {
                logger.LogError("Feedback reply without existed feedback");
                return;
            }

            await publishEndpoint.Publish(new SendNotificationRequest
            {
                UserEmail = feedback.Profile.Email,
                Title = "New reply to your feedback",
                Message = reply.Owner.Email + " sent reply to your feedback"
            });

            await unitOfWork.SaveChangesAsync();
        }
    }
}
