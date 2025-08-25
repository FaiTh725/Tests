using MediatR;
using Notification.Domain.Interfaces;

namespace Notification.Application.Commands.Notifications.ReadAllNotifications
{
    public class ReadAllNotificationsHandler :
        IRequestHandler<ReadAllNotificationsCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public ReadAllNotificationsHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            ReadAllNotificationsCommand request, 
            CancellationToken cancellationToken)
        {
            await unitOfWork.NotificationRepository
                .MarkUserNotificationsAsRead(request.UserEmail, 
                cancellationToken: cancellationToken);
        }
    }
}
