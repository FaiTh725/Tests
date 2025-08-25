using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Notification;
using Test.Application.Contracts.Test;
using Test.Application.Queries.ProfileEntity.Specifications;
using Test.Domain.Enums;
using Test.Domain.Interfaces;

namespace Test.Application.Consumers.TestConsumers
{
    public class TestAccessNotificationConsumer :
        IConsumer<TestAccessNotificationRequest>
    {
        private readonly ILogger<TestAccessNotificationConsumer> logger;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly INoSQLUnitOfWork unitOfWork;

        public TestAccessNotificationConsumer(
            ILogger<TestAccessNotificationConsumer> logger,
            IPublishEndpoint publishEndpoint,
            INoSQLUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
            this.unitOfWork = unitOfWork;
        }

        public async Task Consume(
            ConsumeContext<TestAccessNotificationRequest> context)
        {
            var testAccess = await unitOfWork.AccessRepository
                .GetTestAccess(context.Message.TestAccessId);

            if (testAccess is null)
            {
                logger.LogError("Test access doesnt exist");
                return;
            }

            var test = await unitOfWork.TestRepository
                .GetTest(testAccess.TestId);

            if (test is null)
            {
                logger.LogError("Test from test access doesnt exist");
                return;
            }

            var sendNotificationsTasks = new List<Task>();

            if (testAccess.TargetAccessEntityType == TargetAccessEntityType.Profile)
            {
                var profile = await unitOfWork.ProfileRepository
                    .GetProfile(testAccess.TargetEntityId);

                if (profile is null)
                {
                    logger.LogError("Profile from test access doesnt exist");
                    return;
                }

                sendNotificationsTasks.Add(publishEndpoint
                    .Publish(new SendNotificationRequest
                    {
                        UserEmail = profile.Email,
                        Title = "New test is available",
                        Message = $"The test {test.Name} is now available for you"
                    }));
            }
            else
            {
                var profileGroup = await unitOfWork.ProfileGroupRepository
                    .GetProfileGroup(testAccess.TargetEntityId);

                if (profileGroup is null)
                {
                    logger.LogError("Profile group from test access doesnt exist");
                    return;
                }

                var profiles = await unitOfWork.ProfileRepository
                    .GetProfilesByCriteria(new GetProfilesByIdListSpecification(
                        profileGroup.MembersId));

                var notifyProfilesTassk = profiles.Select(x => publishEndpoint
                    .Publish(new SendNotificationRequest
                    {
                        Message = $"The test {test.Name} is now available for you",
                        Title = "New test is available",
                        UserEmail = x.Email
                    }));

                sendNotificationsTasks.AddRange(notifyProfilesTassk);
            }

            await Task.WhenAll(sendNotificationsTasks);
        }
    }
}
