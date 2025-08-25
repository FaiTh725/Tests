using Application.Shared.Exceptions;
using Authorization.Application.Behaviors;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Common.Interfaces;
using CSharpFunctionalExtensions;
using FluentAssertions;
using MediatR;
using Moq;

namespace Authorization.Application.UnitTests.Users.Behaviors
{
    public class RegistrationAccessBehaviorTests
    {
        private readonly Mock<ICacheService> cacheServiceMock;

        private readonly RegistrationAccessBehavior<RegisterCommand, string> pipelineBehavior;

        public RegistrationAccessBehaviorTests()
        {
            cacheServiceMock = new();

            pipelineBehavior = new RegistrationAccessBehavior<RegisterCommand, string>(
                cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmailIsntConfirmed_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var command = new RegisterCommand
            {
                Email = "sasha.zelenukho.2016@mail.ru",
                Password = "password123",
                UserName = "Faith"
            };

            cacheServiceMock.Setup(x => x
                .GetData<string>(
                    "confirmed_email:" + command.Email, 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<string>("Failure"));

            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            var act = async () => await pipelineBehavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ForbiddenAccessException>()
                .WithMessage("Confirm email before registration");
        }

        [Fact]
        public async Task Handle_WhenEmailConfirmed_ShouldExecuteNextLogic()
        {
            // Arrange
            var command = new RegisterCommand
            {
                Email = "sasha.zelenukho.2016@mail.ru",
                Password = "password123",
                UserName = "Faith"
            };
            var expectedResult = "Something";

            cacheServiceMock.Setup(x => x
                .GetData<string>(
                    "confirmed_email:" + command.Email,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(expectedResult));

            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");

            // Act
            var nextResponce = await pipelineBehavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            nextResponce.Should().Be(expectedResult);
            cacheServiceMock.Verify(x => x
                .GetData<string>(
                    It.IsAny<string>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
