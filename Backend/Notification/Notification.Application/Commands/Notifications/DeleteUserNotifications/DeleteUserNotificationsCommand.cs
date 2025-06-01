using MediatR;
using Notification.Application.Behaviors.Interfaces;

namespace Notification.Application.Commands.Notifications.DeleteUserNotifications
{
    public class DeleteUserNotificationsCommand :
        IRequest,
        IUserNotificationsChanged
    {
        public string UserEmail { get; set; } = string.Empty;

        public List<long> NotificationsId { get; set; } = new List<long>();
    }
}
