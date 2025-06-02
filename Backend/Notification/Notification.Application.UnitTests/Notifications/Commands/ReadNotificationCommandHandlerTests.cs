using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Notification.Application.Commands.Notifications.ReadNotification;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces;
using Notification.Domain.Primitives;
using Notification.Domain.Repositories;

namespace Notification.Application.UnitTests.Notifications.Commands
{
    public class ReadNotificationCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<INotificationRepository> notificationRepositoryMock;

        private readonly ReadNotificationHandler handler;

        public ReadNotificationCommandHandlerTests()
        {
            unitOfWorkMock = new();
            notificationRepositoryMock = new();

            handler = new(unitOfWorkMock.Object);

            unitOfWorkMock
                .Setup(x => x.NotificationRepository)
                .Returns(notificationRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenNotificationDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new ReadNotificationCommand
            {
                NotificationId = 1,
                UserEmail = "test@mail.com"
            };

            notificationRepositoryMock.Setup(x => x
                .GetNotification(command
                    .NotificationId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as NotificationEntity);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Notification doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenUserIsntRecipient_ShouldThrowConflictException()
        {
            // Arrange
            var existedNotification = NotificationEntity.Initialize(
                "another.owner@mail.ru", "title", "message").Value;

            var command = new ReadNotificationCommand
            {
                NotificationId = 1,
                UserEmail = "test@mail.com"
            };

            notificationRepositoryMock.Setup(x => x
                .GetNotification(command
                    .NotificationId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedNotification);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Only owner has access to this notification");
        }

        [Fact]
        public async Task Handle_WhenCommandIsCorrect_ShouldUpdateNotification()
        {
            // Arrange
            var existedNotification = NotificationEntity.Initialize(
                "test@mail.com", "title", "message").Value;

            var command = new ReadNotificationCommand
            {
                NotificationId = 1,
                UserEmail = "test@mail.com"
            };

            notificationRepositoryMock.Setup(x => x
                .GetNotification(command
                    .NotificationId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedNotification);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            notificationRepositoryMock.Verify(x => x
                .GetNotification(
                    command.NotificationId, 
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            notificationRepositoryMock.Verify(x => x
                .UpdateNotification(
                    It.IsAny<long>(),
                    It.IsAny<NotificationEntity>(),
                    It.IsAny<IDatabaseTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
