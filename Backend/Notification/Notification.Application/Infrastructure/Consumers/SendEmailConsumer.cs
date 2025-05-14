using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.DTO;
using Notification.Application.Interfaces;
using Notification.Contracts.Email;

namespace Notification.Application.Infrastructure.Consumers
{
    public class SendEmailConsumer :
        IConsumer<SendEmailRequest>
    {
        private readonly IEmailService emailService;
        private readonly ILogger<SendEmailConsumer> logger;

        public SendEmailConsumer(
            IEmailService emailService,
            ILogger<SendEmailConsumer> logger)
        {
            this.emailService = emailService;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<SendEmailRequest> context)
        {
            if(string.IsNullOrWhiteSpace(context.Message.Consumer) ||
                string.IsNullOrWhiteSpace(context.Message.Subject) ||
                string.IsNullOrWhiteSpace(context.Message.Message))
            {
                logger.LogError("Request message has empty values");
                return;
            }

            await emailService.SendEmail(new EmailDTO
            {
                Email = context.Message.Consumer,
                Message = context.Message.Message,
                Subject  = context.Message.Subject
            });
        }
    }
}
