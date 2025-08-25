using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Email;
using TestRating.Application.Contacts.Feedback;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Consumers.FeedbacksConsumers
{
    public class SendEmailFeedbackDeletedConsumer :
        IConsumer<FeedbackDeletedRequest>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ILogger<SendEmailFeedbackDeletedConsumer> logger;

        public SendEmailFeedbackDeletedConsumer(
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            ILogger<SendEmailFeedbackDeletedConsumer> logger)
        {
            this.unitOfWork = unitOfWork;
            this.publishEndpoint = publishEndpoint;
            this.logger = logger;
        }

        public async Task Consume(
            ConsumeContext<FeedbackDeletedRequest> context)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackExcludeFiltersById(context.Message.FeedbackId);

            if (feedback is null)
            {
                logger.LogError($"Feedback with id = {context.Message.FeedbackId} doesnt exist");
                return;
            }

            var feedbackOwner = await unitOfWork.ProfileRepository
                .GetProfileById(feedback.OwnerId);

            if (feedbackOwner is null)
            {
                logger.LogError($"Feedback owner doesnt not exist");
                return;
            }

            await publishEndpoint.Publish(new SendEmailRequest
            {
                Consumer = feedbackOwner.Email,
                Subject = "Feedback is deleted",
                Message = "Administrator decided to delete your feedback, " +
                "because it violate the rules"
            });

            await unitOfWork.SaveChangesAsync();
        }
    }
}
