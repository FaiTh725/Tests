using MediatR;
using Notification.Application.Behaviors.Interfaces;

namespace Notification.Application.Commands.Notifications.ReadNotification
{
    public class ReadNotificationCommand :
        IRequest,
        IUserNotificationsChanged
    {
        public string UserEmail { get; set; } = string.Empty;

        public long NotificationId { get; set; }
    }
}
