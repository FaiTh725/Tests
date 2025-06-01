using MediatR;
using Notification.Application.Behaviors.Interfaces;
using Notification.Application.DTOs.Notifications;
using Notification.Application.Interfaces;
using Notification.Application.Queries.Notifications.GetUnreadNotifications;

namespace Notification.Application.Behaviors
{
    public class NotifyUserNotificationsChangedBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IUserNotificationsChanged
    {
        private readonly INotificationService notificationService;
        private readonly IMediator mediator;

        public NotifyUserNotificationsChangedBehavior(
            INotificationService notificationService, 
            IMediator mediator)
        {
            this.notificationService = notificationService;
            this.mediator = mediator;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            const int page = 1;
            const int pageSize = 10;

            var result = await next(cancellationToken);

            var notifications = await mediator.Send(new GetUnreadNotificationsQuery
            {
                UserEmail = request.UserEmail,
                Page = page,
                PageSize = pageSize
            }, cancellationToken);

            await notificationService
                .NotifyNotificationsChanged(
                request.UserEmail, 
                notifications.Select(x => new NotificationDTO
                {
                    Id = x.Id,
                    ConsumerEmail = x.ConsumerEmail,
                    Message = x.Message,
                    Title = x.Title
                })
                .ToList());

            return result;
        }
    }
}
