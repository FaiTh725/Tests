using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TestRating.Application.Commands.ProfileEntity.DeleteProfile;
using TestRating.Contracts.Profile;

namespace TestRating.Application.Consumers.ProfileConsumers
{
    public class DeleteFeedbackProfileConsumer : IConsumer<DeleteFeedbackProfile>
    {
        private readonly IMediator mediator;
        private readonly ILogger<DeleteFeedbackProfileConsumer> logger;

        public DeleteFeedbackProfileConsumer(
            IMediator mediator,
            ILogger<DeleteFeedbackProfileConsumer> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<DeleteFeedbackProfile> context)
        {
            try
            {
                await mediator.Send(new DeleteProfileCommand
                {
                    Id = context.Message.Id,
                }, context.CancellationToken);

                logger.LogInformation($"Profile with id {context.Message.Id} deleted");
            }
            catch
            {
                logger.LogError("Error when deleting a profile");
            }
        }
    }
}
