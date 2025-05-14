using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Test.Application.Contracts.Test;
using Test.Domain.Events;

namespace Test.Application.EventHandler.TestEventHandler
{
    public class TestDeletedEventHandler : 
        INotificationHandler<TestDeletedEvent>
    {
        private readonly IPublishEndpoint bus;
        private readonly ILogger<TestDeletedEventHandler> logger;

        public TestDeletedEventHandler(
            IPublishEndpoint bus,
            ILogger<TestDeletedEventHandler> logger)
        {
            this.bus = bus;
            this.logger = logger;
        }

        public async Task Handle(
            TestDeletedEvent notification,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Test deleted event handler starts executing");

            await bus.Publish(new DeleteDependentsTestEntities
            {
                TestId = notification.TestId,
            }, cancellationToken);
        }
    }
}
