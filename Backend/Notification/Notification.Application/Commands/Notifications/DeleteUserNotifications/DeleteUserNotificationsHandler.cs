using Application.Shared.Exceptions;
using MediatR;
using Notification.Application.Queries.Notifications.Specifications;
using Notification.Domain.Interfaces;

namespace Notification.Application.Commands.Notifications.DeleteUserNotifications
{
    public class DeleteUserNotificationsHandler :
        IRequestHandler<DeleteUserNotificationsCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteUserNotificationsHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteUserNotificationsCommand request, 
            CancellationToken cancellationToken)
        {
            var notificationsToDelete = await unitOfWork.NotificationRepository
                .GetNotificationsByCriteria(
                    new GetNotificationsByIdListSpecifications(request.NotificationsId), 
                cancellationToken);

            foreach(var notification in notificationsToDelete)
            {
                if(notification.UserEmail != request.UserEmail)
                {
                    throw new BadRequestException("Any notifications contains a different sender");
                }
            }

            await unitOfWork.NotificationRepository
                .DeleteNotifications(
                    request.NotificationsId, 
                    cancellationToken: cancellationToken);
        }
    }
}
