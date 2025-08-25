using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Notification;
using Test.Domain.Events;
using Test.Domain.Interfaces;

namespace Test.Application.EventHandler.ProfileGroupEventHandler
{
    public class NewMemberAddedEventHandler :
        INotificationHandler<AddedNewGroupMemberEvent>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly ILogger<NewMemberAddedEventHandler> logger;
        private readonly IPublishEndpoint publishEndpoint;

        public NewMemberAddedEventHandler(
            INoSQLUnitOfWork unitOfWork,
            ILogger<NewMemberAddedEventHandler> logger,
            IPublishEndpoint bus)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.publishEndpoint = bus;
        }

        public async Task Handle(
            AddedNewGroupMemberEvent notification, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(notification.ProfileId, cancellationToken);

            if(profile is null)
            {
                logger.LogError("Profile doesnt exist. " +
                    "Impossible to send notification to nonexistent email");
                return;
            }

            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(notification.GroupId, cancellationToken);

            if(group is null)
            {
                logger.LogError($"Group with id={notification.GroupId} doesnt exist");
                return;
            }

            await publishEndpoint.Publish(new SendNotificationRequest
            {
                UserEmail = profile.Email,
                Message = $"You've been added to the group {group.GroupName}",
                Title = $"Welcome to {group.GroupName}"
            },
            cancellationToken);
        }
    }
}
