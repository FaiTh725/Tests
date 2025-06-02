using FluentAssertions;
using Moq;
using Notification.Application.DTOs.Notifications;
using Notification.Application.Queries.Notifications.GetUnreadNotifications;
using Notification.Application.Queries.Notifications.Specifications;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces;
using Notification.Domain.Repositories;

namespace Notification.Application.UnitTests.Notifications.Queries
{
    public class GetUnreadNotificationQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<INotificationRepository> notificationRepositoryMock;

        private readonly GetUnreadNotificationsHandler handler;

        public GetUnreadNotificationQueryHandlerTests()
        {
            unitOfWorkMock = new();
            notificationRepositoryMock = new();

            handler = new(unitOfWorkMock.Object);

            unitOfWorkMock
                .Setup(x => x.NotificationRepository)
                .Returns(notificationRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenAllMessageRead_ShouldReturnsEmptyList()
        {
            // Arrange
            var query = new GetUnreadNotificationsQuery
            {
                UserEmail = "test@mail.com",
                Page = 1,
                PageSize = 12
            };

            notificationRepositoryMock.Setup(x => x
                .GetNotificationsByCriteria(
                    It.IsAny<GetUnreadNotificationsByUserEmailSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenThereUnreadMessages_ShouldReturnsUnreadMessages()
        {
            // Arrange
            var query = new GetUnreadNotificationsQuery
            {
                UserEmail = "test@mail.com",
                Page = 1,
                PageSize = 12
            };

            var existedNotification1 = NotificationEntity
                .Initialize("test@mail.com", "title", "message").Value;
            // set Id
            var type = typeof(NotificationEntity);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedNotification1, [1]);
            // set SendTime
            property = type.GetProperty("SendTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedNotification1, [new DateTime(2025, 5, 10)]);

            var existedNotification2 = NotificationEntity
                .Initialize("test@mail.com", "title", "message").Value;
            // set Id
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedNotification2, [2]);
            // set SendTime
            property = type.GetProperty("SendTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedNotification2, [new DateTime(2025, 5, 10)]);

            notificationRepositoryMock.Setup(x => x
                .GetNotificationsByCriteria(
                    It.IsAny<GetUnreadNotificationsByUserEmailSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedNotification1, existedNotification2]);

            var expectedResult = new List<NotificationDTO>()
            {
                new NotificationDTO
                {
                    Id = 1,
                    ConsumerEmail = "test@mail.com",
                    Message = "message",
                    Title = "title",
                    IsRead = false,
                    SendTime = new DateTime(2025, 5, 10)
                },
                new NotificationDTO
                {
                    Id = 2,
                    ConsumerEmail = "test@mail.com",
                    Message = "message",
                    Title = "title",
                    IsRead = false,
                    SendTime = new DateTime(2025, 5, 10)
                }
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
