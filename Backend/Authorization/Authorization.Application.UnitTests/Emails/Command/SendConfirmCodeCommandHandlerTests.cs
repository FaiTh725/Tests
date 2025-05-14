using Application.Shared.Exceptions;
using Authorization.Application.Commands.Email.SendConfirmCode;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using FluentAssertions;
using MassTransit;
using Moq;
using Notification.Contracts.Email;

namespace Authorization.Application.UnitTests.Emails.Command
{
    public class SendConfirmCodeCommandHandlerTests
    {
        private readonly Mock<ICacheService> cacheServiceMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IBus> busMock;
        private readonly SendConfirmCodeHandler handler;

        public SendConfirmCodeCommandHandlerTests()
        {
            cacheServiceMock = new();
            unitOfWorkMock = new();
            userRepositoryMock = new();
            busMock = new();

            handler = new SendConfirmCodeHandler(
                cacheServiceMock.Object,
                unitOfWorkMock.Object,
                busMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmailIsRegistered_ShouldThrowConflicException()
        {
            // Arrange
            var command = new SendConfirmCodeCommand 
            { 
                Email = "zelenukho725@mail.ru"
            };
            var existedUser = User.Initialize("Faith", "zelenukho725@mail.ru", "2rfsd124", "User").Value;

            userRepositoryMock.Setup(x => x
                .GetUserByEmail(
                    command.Email, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedUser);

            unitOfWorkMock
                .Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Email already registered");
        }

        [Fact]
        public async Task Handle_WhenEmailIsntRegistered_ShouldSendEmailWithCodeAndPutEmailToCache()
        {
            // Arrange
            var command = new SendConfirmCodeCommand
            {
                Email = "zelenukho725@mail.ru"
            };

            userRepositoryMock.Setup(x => x
                .GetUserByEmail(
                    command.Email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            unitOfWorkMock
                .Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            cacheServiceMock.Verify(x => x.SetData(
                It.IsAny<string>(), 
                It.IsAny<UserCode>(), 
                It.IsAny<int>(), 
                It.IsAny<CancellationToken>()),
                Times.Once);
            busMock.Verify(x => x.Publish(
                It.IsAny<SendEmailRequest>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
