using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Notification.Application.Commands.Notifications.DeleteUserNotifications;
using Notification.Application.Queries.Notifications.Specifications;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces;
using Notification.Domain.Primitives;
using Notification.Domain.Repositories;

namespace Notification.Application.UnitTests.Notifications.Commands
{
    public class DeleteNotificationCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<INotificationRepository> notificationRepositoryMock;

        private readonly DeleteUserNotificationsHandler handler;

        public DeleteNotificationCommandHandlerTests()
        {
            unitOfWorkMock = new();
            notificationRepositoryMock = new();

            handler = new(unitOfWorkMock.Object);

            unitOfWorkMock
                .Setup(x => x.NotificationRepository)
                .Returns(notificationRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenNotificationsHaveDifRecipient_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteUserNotificationsCommand
            {
                NotificationsId = [1, 2],
                UserEmail = "test@mail.com"
            };

            var existedNotificationOwnerFromCommand = NotificationEntity
                .Initialize("test@mail.com", "title", "message").Value;

            var existedNotificationStrangeOwner = NotificationEntity
                .Initialize("stange.test@mail.com", "title", "message").Value;

            notificationRepositoryMock.Setup(x => x
                .GetNotificationsByCriteria(
                    It.IsAny<GetNotificationsByIdListSpecifications>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([
                    existedNotificationStrangeOwner,
                    existedNotificationOwnerFromCommand]);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Any notifications contains a different sender");
        }

        [Fact]
        public async Task Handle_WhenNotificationsHaveCommandRecipient_ShouldDeleteNotifications()
        {
            // Arrange
            var command = new DeleteUserNotificationsCommand
            {
                NotificationsId = [1, 2],
                UserEmail = "test@mail.com"
            };

            var existedNotificationOwnerFromCommand = NotificationEntity
                .Initialize("test@mail.com", "title", "message").Value;

            var existedNotificationStrangeOwner = NotificationEntity
                .Initialize("test@mail.com", "title", "message").Value;

            notificationRepositoryMock.Setup(x => x
                .GetNotificationsByCriteria(
                    It.IsAny<GetNotificationsByIdListSpecifications>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([
                    existedNotificationStrangeOwner,
                    existedNotificationOwnerFromCommand]);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            notificationRepositoryMock.Verify(x => x
                .GetNotificationsByCriteria(
                    It.IsAny<GetNotificationsByIdListSpecifications>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            notificationRepositoryMock.Verify(x => x
                .DeleteNotifications(
                    command.NotificationsId, 
                    It.IsAny<IDatabaseTransaction>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
