using Application.Shared.Exceptions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Notification.Application.Configurations;
using Notification.Application.DTO;
using Notification.Application.Interfaces;

namespace Notification.Application.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;
        private readonly EmailServiceConf emailServiceConf;

        public EmailService(
            ILogger<EmailService> logger,
            IConfiguration configuration)
        {
            this.logger = logger;

            emailServiceConf = configuration
                .GetSection("EmailSettings")
                .Get<EmailServiceConf>() ??
                throw new AppConfigurationException("EmailSettings configuration");
        }

        public async Task SendEmail(EmailDTO email)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(
                "Testing",
                emailServiceConf.ReceiverEmail));
            emailMessage.To.Add(new MailboxAddress(
                "",
                email.Email));
            emailMessage.Subject = email.Subject;
            emailMessage.Body = new TextPart()
            { 
                Text = email.Message
            };

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.mail.ru", 465);
                await client.AuthenticateAsync(
                    emailServiceConf.ReceiverEmail,
                    emailServiceConf.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                logger.LogInformation("Email successfully sent to " + email.Email);
            }
            catch
            {
                logger.LogError("Email sent to " + email.Email + " with error");
            }
        }
    }
}
