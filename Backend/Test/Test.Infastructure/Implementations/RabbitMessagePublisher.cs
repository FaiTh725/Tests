using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Test.Application;
using Test.Domain.Interfaces;

namespace Test.Infrastructure.Implementations
{
    public class RabbitMessagePublisher : IMessagePublisher
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IPublishEndpoint bus;
        private readonly ILogger<RabbitMessagePublisher> logger;

        public RabbitMessagePublisher(
            IServiceScopeFactory serviceScopeFactory,
            IPublishEndpoint bus,
            ILogger<RabbitMessagePublisher> logger)
        {
            this.bus = bus;
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task PublishPendingMessages()
        {
            var scope = serviceScopeFactory.CreateAsyncScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<INoSQLUnitOfWork>();

            var pendingMessages = await unitOfWork.OutboxMessageRepository
                .GetPendingMessages();
        
            foreach(var outboxMessage in pendingMessages)
            {
                try
                {
                    var messageType = Type.GetType(outboxMessage.Type);

                    if(messageType is null)
                    {
                        throw new NullReferenceException("Failed to get message type");
                    }

                    var deserialisedMessage = JsonSerializer.Deserialize(outboxMessage.Payload, messageType);

                    if(deserialisedMessage is null)
                    {
                        throw new JsonException("An error occurred deserialize message");
                    }

                    await bus.Publish(deserialisedMessage);

                    outboxMessage.ProcessMessage();

                    await unitOfWork.OutboxMessageRepository
                        .UpdateMessage(outboxMessage.Id, outboxMessage);
                }
                catch (Exception ex)
                {
                    logger.LogInformation(ex, "Message publisher was executed with an error - " +
                        ex.Message);
                    throw;
                }
            }
        }
    }
}
