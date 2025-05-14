using Application.Shared.Exceptions;
using Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken;
using Authorization.Application.Common.Implementations;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using FluentAssertions;
using MediatR;
using Moq;

namespace Authorization.Application.UnitTests.Users.Services
{
    public class AuthServiceRefreshTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IRefreshTokenRepository> refreshTokenRepositoryMock;
        private readonly Mock<IMediator> mediatorMock;
        private readonly Mock<IJwtService<UserTokenRequest, UserTokenResponse>> tokenServiceMock;

        private readonly IAuthService authService;

        private readonly User user;

        public AuthServiceRefreshTests()
        {
            unitOfWorkMock = new();
            mediatorMock = new();
            tokenServiceMock = new();
            refreshTokenRepositoryMock = new();

            authService = new AuthService(
                unitOfWorkMock.Object,
                tokenServiceMock.Object,
                mediatorMock.Object);

            user = User.Initialize("gfdgdfh34d", "zelenukho725@mail.ru", "fsq34tsd1234", "User").Value;
            var type = typeof(User);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(user, [1]);
        }

        [Fact]
        public async Task Refresh_WhenRefreshTokenIsEmpty_ShouldThrowUnauthorizeException()
        {
            // Arrange
            string? refreshToken = null;

            // Act
            var act = async () => await authService.Refresh(refreshToken, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizeException>()
                .WithMessage("Refresh token is missing");
        }

        [Fact]
        public async Task Refresh_WhenRefreshTokenDoesntExist_ShouldThrowUnauthorizeException()
        {
            // Arrange
            var oldRefreshToken = "df124fsyh6";

            refreshTokenRepositoryMock.Setup(x => x
                .GetRefreshTokenWithUser(
                    oldRefreshToken, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as RefreshToken);

            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            // Act
            var act = async () => await authService.Refresh(oldRefreshToken, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizeException>()
                .WithMessage("Refresh token is missing or expired");
        }

        [Fact]
        public async Task Refresh_WhenRefreshTokenIsExpired_ShouldThrowUnauthorizeException()
        {
            // Arrange
            var oldRefreshToken = "fsdf124gfd346";
            var refreshToken = RefreshToken.Initialize(
                    oldRefreshToken, user, 
                    DateTime.UtcNow.AddDays(1)).Value;
            // set expire on date to the past
            var type = typeof(RefreshToken);
            var property = type.GetProperty("ExpireOn");
            var setMethod = property!.GetSetMethod(true);
            setMethod!.Invoke(refreshToken, [DateTime.UtcNow.AddDays(-1)]);

            refreshTokenRepositoryMock.Setup(x => x
                .GetRefreshTokenWithUser(
                    oldRefreshToken,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            // Act
            var act = async () => await authService.Refresh(oldRefreshToken, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<UnauthorizeException>()
                .WithMessage("Refresh token is missing or expired");
        }

        [Fact]
        public async Task Refresh_WhenRefreshTokenIsntExpiredAndExist_ShouldGenerateNewRefreshTokenAndAccessToken()
        {
            // Arrange
            var oldRefreshToken = "df124fsyh6";
            var newRefreshToken = "123fsghgxasl2";
            var newAccessToken = "fds2144.fdsf23.fwed23t";
            var refreshToken = RefreshToken.Initialize(
                    oldRefreshToken, user,
                    DateTime.UtcNow.AddDays(1)).Value;

            refreshTokenRepositoryMock.Setup(x => x
                .GetRefreshTokenWithUser(
                    oldRefreshToken,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            tokenServiceMock.Setup(x => x
                .GenerateRefreshToken())
                .Returns(newRefreshToken);

            tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<UserTokenRequest>()))
                .Returns(newAccessToken);

            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            // Act
            var newTokensPair = await authService.Refresh(oldRefreshToken, CancellationToken.None);

            // Assert
            newTokensPair.Item1.Should().Be(newAccessToken);
            newTokensPair.Item2.Should().Be(newRefreshToken);
            mediatorMock.Verify(x => x.Send(
                It.IsAny<RefreshRefreshTokenCommand>(), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
            tokenServiceMock.Verify(x => x.GenerateToken(
                It.IsAny<UserTokenRequest>()), 
            Times.Once);
            tokenServiceMock.Verify(x => x.GenerateRefreshToken(), 
            Times.Once);
        }
    }
}
