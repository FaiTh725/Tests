using Application.Shared.Exceptions;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Authorization.Application.UnitTests.Users.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly LoginHandler handler;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IHashService> hashServiceMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IJwtService<UserTokenRequest, UserTokenResponse>> tockenServiceMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> refreshTokenMock;
        private readonly User existUser;
        private readonly RefreshToken oldUserRefreshToken;
        private const int expirationTimeRefreshTokenInDays = 15;

        public LoginCommandHandlerTests()
        {
            unitOfWorkMock = new();
            hashServiceMock = new();
            configurationMock = new();
            tockenServiceMock = new();
            userRepositoryMock = new();
            refreshTokenMock = new();
            existUser = User.Initialize("FaiTh", "sasha.zelenukho.2016@mail.ru", "787ca7e12a2fa", "User").Value;
            oldUserRefreshToken = RefreshToken.Initialize("787ca7e12a2fa", existUser, DateTime.UtcNow.AddDays(3)).Value;

            // set id for exist user
            var type = typeof(User);
            var property = type.GetProperty("Id");
            var setMethod = property!.GetSetMethod(true);
            setMethod!.Invoke(existUser, [1]);

            // set id for old user refresh token
            type = typeof(RefreshToken);
            property = type.GetProperty("Id");
            setMethod = property!.GetSetMethod(true);
            setMethod!.Invoke(oldUserRefreshToken, [1]);

            this.handler = new LoginHandler(
                unitOfWorkMock.Object, hashServiceMock.Object,
                tockenServiceMock.Object, configurationMock.Object);

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock
                .Setup(x => x.Value)
                .Returns(expirationTimeRefreshTokenInDays.ToString());

            configurationMock.Setup(x => x
                .GetSection("JwtSettings:ExpirationTimeRefreshTokenInDays"))
                .Returns(configurationSectionMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmailIsntRegistered_ShouldThrowBadRequestException()
        { 
            // Arrange
            var command = new LoginCommand { Email = "sasha.zelenukho.2016@mail.ru", Password = "string123" };
            userRepositoryMock
                .Setup(x => x.GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage($"Email {command.Email} isnt registered");
        }

        [Fact]
        public async Task Handle_WhenEmailOrPasswordIsIncorrect_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new LoginCommand { Email = existUser.Email, Password = "string123" };
            userRepositoryMock
                .Setup(x => x.GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existUser);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            hashServiceMock.Setup(x => x
                .VerifyHash(command.Password, existUser.PasswordHash))
                .Returns(false);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Email invalid password or email");
        }

        [Fact]
        public async Task Handle_WhenRefreshTokenExist_ShouldUpdateOldRefreshToken()
        {
            // Arrange
            var command = new LoginCommand { Email = existUser.Email, Password = "string123" };
            var newTokenValue = "31fewwq414g";
            userRepositoryMock
                .Setup(x => x.GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existUser);

            hashServiceMock.Setup(x => x
                .VerifyHash(command.Password, existUser.PasswordHash))
                .Returns(true);

            refreshTokenMock.Setup(x => x
                .GetRefreshTokenByUserId(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldUserRefreshToken);

            tockenServiceMock.Setup(x => x
                .GenerateRefreshToken())
                .Returns(newTokenValue);

            unitOfWorkMock.Setup(x => x
            .SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenMock.Object);

            // Act
            var handlerResult = await handler.Handle(command, CancellationToken.None);

            // Assert
            handlerResult.Item1.Should().Be(existUser.Id);
            handlerResult.Item2.Should().Be(newTokenValue);
            unitOfWorkMock.Verify(u => u.RefreshTokenRepository
                .UpdateRefreshToken(
                    oldUserRefreshToken.Id, 
                    oldUserRefreshToken, 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
            unitOfWorkMock.Verify(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()), 
                Times.Once);
            oldUserRefreshToken.Token.Should().Be(newTokenValue);
            oldUserRefreshToken.ExpireOn.Should().BeCloseTo(
                DateTime.UtcNow.AddDays(expirationTimeRefreshTokenInDays), 
                TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Handle_WhenRefreshTokenIsntExist_ShouldAddNewRefreshToken()
        {
            // Arrange
            var command = new LoginCommand { Email = existUser.Email, Password = "string123" };
            var newTokenValue = "31fewwq414g";
            userRepositoryMock
                .Setup(x => x.GetUserByEmail(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existUser);

            hashServiceMock.Setup(x => x
                .VerifyHash(command.Password, existUser.PasswordHash))
                .Returns(true);

            refreshTokenMock.Setup(x => x
                .GetRefreshTokenByUserId(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as RefreshToken);

            tockenServiceMock.Setup(x => x
                .GenerateRefreshToken())
                .Returns(newTokenValue);

            unitOfWorkMock.Setup(x => x
            .SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenMock.Object);

            // Act
            var handlerResult = await handler.Handle(command, CancellationToken.None);

            // Assert
            handlerResult.Item1.Should().Be(existUser.Id);
            handlerResult.Item2.Should().Be(newTokenValue);
            unitOfWorkMock.Verify(u => u
                .SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
            unitOfWorkMock.Verify(u => u
                .RefreshTokenRepository.AddRefreshToken(
                    It.IsAny<RefreshToken>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
