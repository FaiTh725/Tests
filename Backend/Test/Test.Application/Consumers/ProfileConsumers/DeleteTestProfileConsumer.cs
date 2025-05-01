using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Test.Application.Commands.ProfileEntity.DeleteProfile;
using Test.Contracts.Profile;

namespace Test.Application.Consumers.ProfileConsumers
{
    public class DeleteTestProfileConsumer : IConsumer<DeleteTestProfile>
    {
        private readonly ILogger<DeleteTestProfileConsumer> logger;
        private readonly IMediator mediator;

        public DeleteTestProfileConsumer(
            ILogger<DeleteTestProfileConsumer> logger,
            IMediator mediator)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<DeleteTestProfile> context)
        {
            try
            {

                await mediator.Send(new DeleteProfileCommand
                {
                    ProfileId = context.Message.Id
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
