using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Email;
using Test.Domain.Events;
using Test.Domain.Interfaces;

namespace Test.Application.EventHandler.ProfileGroupEventHandler
{
    public class NewMemberAddedEventHandler :
        INotificationHandler<AddedNewGroupMember>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly ILogger<NewMemberAddedEventHandler> logger;
        private readonly IPublishEndpoint bus;

        public NewMemberAddedEventHandler(
            INoSQLUnitOfWork unitOfWork,
            ILogger<NewMemberAddedEventHandler> logger,
            IPublishEndpoint bus)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.bus = bus;
        }

        public async Task Handle(
            AddedNewGroupMember notification, 
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

            await bus.Publish(new SendEmailRequest
            {
                Consumer = profile.Email,
                Subject = "Testing Groups",
                Message = $"You've been added to the group {group.GroupName}"
            },
            cancellationToken);
        }
    }
}
