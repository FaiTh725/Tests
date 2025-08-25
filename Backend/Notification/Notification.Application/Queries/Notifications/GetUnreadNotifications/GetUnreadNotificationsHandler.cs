using MediatR;
using Notification.Application.DTOs.Notifications;
using Notification.Application.Queries.Notifications.Specifications;
using Notification.Domain.Interfaces;

namespace Notification.Application.Queries.Notifications.GetUnreadNotifications
{
    public class GetUnreadNotificationsHandler :
        IRequestHandler<GetUnreadNotificationsQuery, IEnumerable<NotificationDTO>>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetUnreadNotificationsHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificationDTO>> Handle(
            GetUnreadNotificationsQuery request, 
            CancellationToken cancellationToken)
        {
            var notifications = await unitOfWork.NotificationRepository
                .GetNotificationsByCriteria(
                new GetUnreadNotificationsByUserEmailSpecification(
                        request.UserEmail, 
                        request.Page, 
                        request.PageSize), 
                cancellationToken);

            return notifications.Select(x => new NotificationDTO
            {
                Id = x.Id,
                ConsumerEmail = x.UserEmail,
                Message = x.Message,
                Title = x.Title,
                SendTime = x.SendTime,
                IsRead = x.IsRead
            });
        }
    }
}
