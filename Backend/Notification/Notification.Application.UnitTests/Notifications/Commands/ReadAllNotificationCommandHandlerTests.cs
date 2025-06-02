using Moq;
using Notification.Application.Commands.Notifications.ReadAllNotifications;
using Notification.Domain.Interfaces;
using Notification.Domain.Primitives;
using Notification.Domain.Repositories;

namespace Notification.Application.UnitTests.Notifications.Commands
{
    public class ReadAllNotificationCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<INotificationRepository> notificationRepositoryMock;

        private readonly ReadAllNotificationsHandler handler;

        public ReadAllNotificationCommandHandlerTests()
        {
            unitOfWorkMock = new();
            notificationRepositoryMock = new();

            handler = new(unitOfWorkMock.Object);

            unitOfWorkMock
                .Setup(x => x.NotificationRepository)
                .Returns(notificationRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_VerifyMethodWasCalled()
        {
            // Arrange
            var command = new ReadAllNotificationsCommand
            {
                UserEmail = "test@mail.com"
            };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            notificationRepositoryMock.Verify(x => x
                .MarkUserNotificationsAsRead(
                    command.UserEmail, 
                    It.IsAny<IDatabaseTransaction>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
