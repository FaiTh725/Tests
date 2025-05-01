using Application.Shared.Exceptions;
using Authorization.Contracts.Events.Profile;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Test.Application.Commands.ProfileEntity.CreateProfile;
using Test.Contracts.Profile;

namespace Test.Application.Consumers.ProfileConsumers
{
    public class CreateTestProfileConsumer : IConsumer<CreateTestProfile>
    {
        private readonly ILogger<CreateTestProfileConsumer> logger;
        private readonly IMediator mediator;

        public CreateTestProfileConsumer(
            ILogger<CreateTestProfileConsumer> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        public async Task Consume(ConsumeContext<CreateTestProfile> context)
        {
            try
            {
                var profileId = await mediator.Send(new CreateProfileCommand
                {
                    Email = context.Message.Email,
                    Name = context.Message.Name,
                }, context.CancellationToken);

                logger.LogInformation("Profile created");

                await context.Publish<ITestProfileCreated>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    Email = context.Message.Email,
                    Name = context.Message.Name,
                    TestProfileId = profileId
                }, context.CancellationToken);
            }
            catch(ApiException apiEx)
            {
                logger.LogError("Creating profile with error - " +
                    apiEx.Message);

                await context.Publish<ITestProfileCreateFailed>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    Email = context.Message.Email,
                    Name = context.Message.Name,
                    Reason = apiEx.Message
                });
            }
            catch (Exception)
            {
                logger.LogError("Creating profile with error");

                await context.Publish<ITestProfileCreateFailed>(new
                {
                    CorrelationId = context.Message.CorrelationId,
                    Email = context.Message.Email,
                    Name = context.Message.Name,
                    Reason = "Unexpected error"
                });
            }
        }
    }
}
