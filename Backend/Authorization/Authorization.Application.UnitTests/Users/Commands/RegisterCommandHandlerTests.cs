using Application.Shared.Exceptions;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using Authorization.Domain.Validators;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Authorization.Application.UnitTests.Users.Commands
{
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> refreshTokenRepositoryMock;
        private readonly Mock<IHashService> hashServiceMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IBus> busMock;
        private readonly Mock<IJwtService<UserTokenRequest, UserTokenResponse>> tokenServiceMock;
        private readonly RegisterHandler handler;

        private const int expirationTimeRefreshTokenInDays = 15;

        public RegisterCommandHandlerTests()
        {
            unitOfWorkMock = new();
            hashServiceMock = new();
            configurationMock = new();
            busMock = new();
            tokenServiceMock = new();
            userRepositoryMock = new();
            refreshTokenRepositoryMock = new();

            handler = new RegisterHandler(
                unitOfWorkMock.Object, 
                hashServiceMock.Object, 
                tokenServiceMock.Object, 
                configurationMock.Object, 
                busMock.Object);

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock
                .Setup(x => x.Value)
                .Returns(expirationTimeRefreshTokenInDays.ToString());

            configurationMock.Setup(x => x
                .GetSection("JwtSettings:ExpirationTimeRefreshTokenInDays"))
                .Returns(configurationSectionMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyRegistered_ShouldThrowConflictException()
        {
            // Arrange
            var command = new RegisterCommand() 
            { 
                Email = "zelenukho725@mail.com", 
                Password = "string123", 
                UserName = "Faith"
            };
            var existedUser = User.Initialize("FaITh", "zelenukho725@mail.com", "fdasf1234", "User").Value;
            userRepositoryMock.Setup(x => x
                    .GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedUser);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("User already registered");
        }

        [Fact]
        public async Task Handle_WhenOutOfBoundsLengthPassword_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new RegisterCommand()
            {
                Email = "zelenukho725@mail.com",
                Password = "123",
                UserName = "Faith"
            };

            userRepositoryMock.Setup(x => x
                    .GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Password must has one letter and one number, " +
                    $"in range size from {UserValidator.MIN_PASSWORD_LENGTH} to {UserValidator.MAX_PASSWORD_LENGTH}");
        }

        [Fact]
        public async Task Handle_WhenPasswordDoesntContainNumber_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new RegisterCommand()
            {
                Email = "zelenukho725@mail.com",
                Password = "string",
                UserName = "Faith"
            };

            userRepositoryMock.Setup(x => x
                    .GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Password must has one letter and one number, " +
                    $"in range size from {UserValidator.MIN_PASSWORD_LENGTH} to {UserValidator.MAX_PASSWORD_LENGTH}");
        }

        [Fact]
        public async Task Handle_WhenPasswordDoesntContainLetter_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new RegisterCommand()
            {
                Email = "zelenukho725@mail.com",
                Password = "7252716600",
                UserName = "Faith"
            };

            userRepositoryMock.Setup(x => x
                    .GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Password must has one letter and one number, " +
                    $"in range size from {UserValidator.MIN_PASSWORD_LENGTH} to {UserValidator.MAX_PASSWORD_LENGTH}");
        }

        [Fact]
        public async Task Handle_CommandIsCorrect_ShouldRegisterUser()
        {
            // Arrange
            var command = new RegisterCommand()
            {
                Email = "zelenukho725@mail.com",
                Password = "string123",
                UserName = "Faith"
            };
            var refreshToken = "fd124gw462";
            var expectedUserFromDb = User.Initialize(
                "Faith", "zelenukho725@mail.com", 
                "124edftywf", "User").Value;
            // set userId instead of db
            var type = typeof(User);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod!.Invoke(expectedUserFromDb, [1]);

            userRepositoryMock.Setup(x => x
                    .GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);
            userRepositoryMock.Setup(x => x
                    .AddUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUserFromDb);
            hashServiceMock.Setup(x => x
                    .GenerateHash(command.Password))
                .Returns(expectedUserFromDb.PasswordHash);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            tokenServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns(refreshToken);

            // Act
            var handlerResult = await handler.Handle(command, CancellationToken.None);

            // Assert
            handlerResult.Item2.Should().Be(refreshToken);
            handlerResult.Item1.Should().Be(expectedUserFromDb.Id);

            userRepositoryMock.Verify(x => x
                .AddUser(
                    It.IsAny<User>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
            refreshTokenRepositoryMock.Verify(x => x
                .AddRefreshToken(
                    It.IsAny<RefreshToken>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
