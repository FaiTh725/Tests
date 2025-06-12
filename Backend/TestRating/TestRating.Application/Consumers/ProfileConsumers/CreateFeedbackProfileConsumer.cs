using Application.Shared.Exceptions;
using Authorization.Contracts.Events.Profile;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TestRating.Application.Commands.ProfileEntity.AddProfile;
using TestRating.Contracts.Profile;

namespace TestRating.Application.Consumers.ProfileConsumers
{
    public class CreateFeedbackProfileConsumer : IConsumer<CreateFeedbackProfile>
    {
        private readonly ILogger<CreateFeedbackProfileConsumer> logger;
        private readonly IMediator mediator;

        public CreateFeedbackProfileConsumer(
            ILogger<CreateFeedbackProfileConsumer> logger,
            IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        public async Task Consume(ConsumeContext<CreateFeedbackProfile> context)
        {
            try
            {
                var profileId = await mediator.Send(new AddProfileCommand 
                { 
                    Email = context.Message.Email,
                    Name = context.Message.Name
                });

                await context.Publish<IFeedbackProfileCreated>(new 
                {
                    CorrelationId = context.Message.CorrelationId,
                    FeedbackProfileId = profileId,
                    Email = context.Message.Email,
                    Name = context.Message.Name
                });
            }
            catch (ApiException apiEx)
            {
                logger.LogError("Creating profile with error - " +
                    apiEx.Message);

                await context.Publish<IFeedbackProfileCreateFailed>(new
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

                await context.Publish<IFeedbackProfileCreateFailed>(new
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
