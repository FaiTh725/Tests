using MassTransit;
using Notification.Contracts.Email;

namespace Test.Integration.Tests.Consumers
{
    public class MessagesConsumer : IConsumer<SendEmailRequest>
    {
        public Task Consume(ConsumeContext<SendEmailRequest> context)
        {
            // blank consumer, to process messages instead of external services
            return Task.CompletedTask;
        }
    }
}
