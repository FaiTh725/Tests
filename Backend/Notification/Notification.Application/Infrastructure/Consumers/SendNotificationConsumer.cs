using Application.Shared.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.DTOs.Notifications;
using Notification.Application.Interfaces;
using Notification.Contracts.Notification;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces;

namespace Notification.Application.Infrastructure.Consumers
{
    public class SendNotificationConsumer :
        IConsumer<SendNotificationRequest>
    {
        private readonly ILogger<SendNotificationConsumer> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly INotificationService notificationService;

        public SendNotificationConsumer(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<SendNotificationConsumer> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<SendNotificationRequest> context)
        {
            logger.LogInformation("Notification consumer got notification");

            var notificationResult = NotificationEntity.Initialize(
                context.Message.UserEmail, 
                context.Message.Title, 
                context.Message.Message);
        
            if(notificationResult.IsFailure)
            {
                throw new BadRequestException("Error with request - " + 
                    notificationResult.Error);
            }

            var notificationFromDb = await unitOfWork.NotificationRepository
                .AddNotification(notificationResult.Value);

            await notificationService.SendNotification(new NotificationDTO
            {
                Id = notificationFromDb.Id,
                ConsumerEmail = notificationFromDb.UserEmail,
                Title = notificationFromDb.Title,
                Message = notificationFromDb.Message
            });

            logger.LogInformation("Notification consumer processed notification");
        }
    }
}
