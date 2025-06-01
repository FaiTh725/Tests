using Application.Shared.Exceptions;
using MediatR;
using Notification.Domain.Interfaces;

namespace Notification.Application.Commands.Notifications.ReadNotification
{
    public class ReadNotificationHandler :
        IRequestHandler<ReadNotificationCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public ReadNotificationHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(
            ReadNotificationCommand request, 
            CancellationToken cancellationToken)
        {
            var notification = await unitOfWork.NotificationRepository
                .GetNotification(request.NotificationId, cancellationToken);
        
            if(notification is null)
            {
                throw new BadRequestException("Notification doesnt exist");
            }

            if(notification.UserEmail != request.UserEmail)
            {
                throw new ConflictException("Only owner has access to this notification");
            }

            notification.Read();

            await unitOfWork.NotificationRepository
                .UpdateNotification(
                    notification.Id, 
                    notification, 
                    cancellationToken: cancellationToken);
        }
    }
}
