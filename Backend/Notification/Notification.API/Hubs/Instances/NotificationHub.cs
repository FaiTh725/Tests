using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Notification.Application.DTOs.Notifications;
using Notification.Application.Queries.Notifications.GetUnreadNotifications;
using IHubNotification = Notification.API.Hubs.Interfaces.INotification;

namespace Notification.API.Hubs.Instances
{
    [Authorize]
    public class NotificationHub : Hub<IHubNotification>
    {
        private readonly IMediator mediator;

        public NotificationHub(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        public override async Task OnConnectedAsync()
        {
            const int page = 1;
            const int pageSize = 10;

            var userEmail = Context.Items["email"]!.ToString()!;

            var unreadNotifcations = await mediator.Send(new GetUnreadNotificationsQuery
            {
                UserEmail = userEmail,
                Page = page,
                PageSize = pageSize
            });

            await Clients.Caller
                .SendListNotifications(unreadNotifcations.ToList());
        }
    }
}
