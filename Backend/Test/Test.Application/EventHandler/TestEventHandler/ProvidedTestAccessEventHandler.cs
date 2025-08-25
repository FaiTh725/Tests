using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Test.Application.Contracts.Test;
using Test.Domain.Events;

namespace Test.Application.EventHandler.TestEventHandler
{
    public class ProvidedTestAccessEventHandler :
        INotificationHandler<NewTestAccessCreated>
    {
        private readonly ILogger<ProvidedTestAccessEventHandler> logger;
        private readonly IPublishEndpoint publishEndpoint;

        public ProvidedTestAccessEventHandler(
            ILogger<ProvidedTestAccessEventHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            this.logger = logger;
        }

        public async Task Handle(
            NewTestAccessCreated notification, 
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Provide test access handler get message");

            await publishEndpoint.Publish(new TestAccessNotificationRequest
            {
                TestAccessId = notification.TestAccessId,
            }, cancellationToken);
        }
    }
}
