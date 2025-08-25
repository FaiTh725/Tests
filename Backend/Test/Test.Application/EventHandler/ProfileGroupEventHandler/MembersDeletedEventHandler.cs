using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Notification.Contracts.Notification;
using Test.Application.Queries.ProfileEntity.Specifications;
using Test.Domain.Events;
using Test.Domain.Interfaces;

namespace Test.Application.EventHandler.ProfileGroupEventHandler
{
    public class MembersDeletedEventHandler :
        INotificationHandler<MembersDeletedEvent>
    {
        private readonly ILogger<MembersDeletedEventHandler> logger;
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IPublishEndpoint publishEndpoint;

        public MembersDeletedEventHandler(
            ILogger<MembersDeletedEventHandler> logger,
            INoSQLUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Handle(
            MembersDeletedEvent notification, 
            CancellationToken cancellationToken)
        {
            if(notification.MembersId.Count == 0)
            {
                logger.LogError("Nothing to notice");
                return;
            }

            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(notification.GroupId, cancellationToken);

            if(group is null)
            {
                logger.LogError("Group has no longer exist");
                return;
            }

            var profiles = await unitOfWork.ProfileRepository
                .GetProfilesByCriteria(new GetProfilesByIdListSpecification(
                        notification.MembersId),
                cancellationToken);

            var sendNotificationsTasks = profiles.Select(x => 
                publishEndpoint.Publish(new SendNotificationRequest
                {
                    Title = "Happy trails",
                    Message = $"Youve been removed from the group {group.GroupName}",
                    UserEmail = x.Email
                }))
                .ToList();

            await Task.WhenAll(sendNotificationsTasks);
        }
    }
}
