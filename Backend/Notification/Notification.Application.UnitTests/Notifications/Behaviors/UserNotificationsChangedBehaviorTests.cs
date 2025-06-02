using FluentAssertions;
using MediatR;
using Moq;
using Notification.Application.Behaviors;
using Notification.Application.Commands.Notifications.DeleteUserNotifications;
using Notification.Application.DTOs.Notifications;
using Notification.Application.Interfaces;
using Notification.Application.Queries.Notifications.GetUnreadNotifications;

namespace Notification.Application.UnitTests.Notifications.Behaviors
{
    public class UserNotificationsChangedBehaviorTests
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly Mock<INotificationService> notificationServiceMock;

        private readonly NotifyUserNotificationsChangedBehavior<DeleteUserNotificationsCommand, string> behavior;
        private readonly Mock<RequestHandlerDelegate<string>> nextMock;

        public UserNotificationsChangedBehaviorTests()
        {
            mediatorMock = new();
            notificationServiceMock = new();

            behavior = new(
                notificationServiceMock.Object,
                mediatorMock.Object);
            nextMock = new();
        }

        [Fact]
        public async Task Handle_VerifyNotificationsSend()
        {
            // Arrange
            var command = new DeleteUserNotificationsCommand
            {
                NotificationsId = [1, 2],
                UserEmail = "test@mail.com"
            };

            nextMock.Setup(x => x
                .Invoke(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("something");

            mediatorMock.Setup(x => x
                .Send(
                    It.IsAny<GetUnreadNotificationsQuery>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be("something");

            notificationServiceMock.Verify(x => x
                .NotifyNotificationsChanged(
                    command.UserEmail, 
                    It.IsAny<List<NotificationDTO>>()), 
                Times.Once);

            mediatorMock.Verify(x => x
                .Send(
                    It.IsAny<GetUnreadNotificationsQuery>(),
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
