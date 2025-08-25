using MediatR;
using Notification.Application.Behaviors.Interfaces;

namespace Notification.Application.Commands.Notifications.ReadAllNotifications
{
    public class ReadAllNotificationsCommand : 
        IRequest,
        IUserNotificationsChanged
    {
        public string UserEmail { get; set; } = string.Empty;
    }
}
