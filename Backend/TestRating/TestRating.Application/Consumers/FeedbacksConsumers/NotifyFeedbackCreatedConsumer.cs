using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Notification;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Feedback;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Consumers.FeedbacksConsumers
{
    public class NotifyFeedbackCreatedConsumer : 
        IConsumer<FeedbackCreatedRequest>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ITestExternalService testExternalService;
        private readonly ILogger<NotifyFeedbackCreatedConsumer> logger;

        public NotifyFeedbackCreatedConsumer(
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            ITestExternalService testExternalService,
            ILogger<NotifyFeedbackCreatedConsumer> logger)
        {
            this.unitOfWork = unitOfWork;
            this.publishEndpoint = publishEndpoint;
            this.testExternalService = testExternalService;
            this.logger = logger;
        }

        public async Task Consume(
            ConsumeContext<FeedbackCreatedRequest> context)
        {
            logger.LogInformation("Notification Feedback Created Consumer got message");

            var testResult = await testExternalService
                .GetTest(context.Message.TestId);

            if(testResult.IsFailure)
            {
                logger.LogError("Error with notify when feedback was created. " +
                    "Error message " + testResult.Error);
                return;
            }

            await publishEndpoint.Publish(new SendNotificationRequest
            {
                UserEmail = testResult.Value.Owner.Email,
                Title = "Your test has received some new feedback",
                Message = "Some sent feedback on your test named " + testResult.Value.Name
            });

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Notification Feedback Created Consumer sent notification");
        }
    }
}
