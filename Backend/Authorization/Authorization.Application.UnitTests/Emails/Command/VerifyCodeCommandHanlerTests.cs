using Application.Shared.Exceptions;
using Authorization.Application.Commands.Email.VerifyCode;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;

namespace Authorization.Application.UnitTests.Emails.Command
{
    public class VerifyCodeCommandHanlerTests
    {
        private readonly Mock<ICacheService> cacheServiceMock;
        private readonly VerifyCodeHandler handler;

        public VerifyCodeCommandHanlerTests()
        {
            cacheServiceMock = new();

            handler = new VerifyCodeHandler(cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenCodeDoesntExistInCache_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new VerifyCodeCommand() 
            { 
                Email = "zelenukh725@mail.ru", 
                Code = "123521" 
            };
            var result = Result.Failure<UserCode>("Cache doesnt exist");

            cacheServiceMock.Setup(x => x
                .GetData<UserCode>(
                    "unconfirmed_mail:" + command.Email, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Invalid code or email");
        }

        [Fact]
        public async Task Handle_WhenCodeIsInvalid_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new VerifyCodeCommand()
            {
                Email = "zelenukh725@mail.ru",
                Code = "123521"
            };
            var result = Result.Success(new UserCode
            {
                Code = "1244123431",
                SendingTime = DateTime.UtcNow,
            });

            cacheServiceMock.Setup(x => x
                .GetData<UserCode>(
                    "unconfirmed_mail:" + command.Email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Invalid code or email");
        }

        [Fact]
        public async Task Handler_CodeIsCorrect_ShouldSetEmailUsConfirmedAndRemoveFromUnconfirmed()
        {
            // Arrange
            var command = new VerifyCodeCommand()
            {
                Email = "zelenukh725@mail.ru",
                Code = "123521"
            };
            var result = Result.Success(new UserCode
            {
                Code = "123521",
                SendingTime = DateTime.UtcNow,
            });

            cacheServiceMock.Setup(x => x
                .GetData<UserCode>(
                    "unconfirmed_mail:" + command.Email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            cacheServiceMock.Verify(x => x.SetData(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<int>(), 
                It.IsAny<CancellationToken>()), 
                Times.Once);

            cacheServiceMock.Verify(x => x.RemoveData(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
