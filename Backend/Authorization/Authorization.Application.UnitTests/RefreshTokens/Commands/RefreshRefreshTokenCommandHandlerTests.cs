using Application.Shared.Exceptions;
using Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Authorization.Application.UnitTests.RefreshTokens.Commands
{
    public class RefreshRefreshTokenCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IRefreshTokenRepository> refreshTokenRepositoryMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly RefreshRefreshTokenHandler handler;

        private const int expirationTimeRefreshToken = 15;

        public RefreshRefreshTokenCommandHandlerTests()
        {
            unitOfWorkMock = new();
            configurationMock = new();
            refreshTokenRepositoryMock = new();

            handler = new RefreshRefreshTokenHandler(
                unitOfWorkMock.Object, configurationMock.Object);

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock
                .Setup(x => x.Value)
                .Returns(expirationTimeRefreshToken.ToString());

            configurationMock.Setup(x => x
                .GetSection("JwtSettings:ExpirationTimeRefreshTokenInDays"))
                .Returns(configurationSectionMock.Object);
        }

        [Fact]
        public async Task Handler_WhenRefreshTokenDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new RefreshRefreshTokenCommand { Id = 1, NewToken = "dasdqq2gsw45" };

            refreshTokenRepositoryMock.Setup(x => x
            .GetRefreshToken(
                    command.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as RefreshToken);

            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Token doesnt exist");
        }

        [Fact]
        public async Task Handler_WhenRefreshTokenExist_ShouldUpdateRefreshToken()
        {
            // Arrange
            var command = new RefreshRefreshTokenCommand { Id = 1, NewToken = "dfsaew1234tgf" };
            var user = User.Initialize("Faith", "sasha.zelenukho.2016@mail.ru", "fdffwe34", "User").Value;
            var existedRefreshToken = RefreshToken.Initialize("dfssdf325hgfd", user, DateTime.UtcNow.AddDays(15)).Value;
            
            refreshTokenRepositoryMock.Setup(x => x
            .GetRefreshToken(
                    command.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedRefreshToken);

            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            refreshTokenRepositoryMock.Verify(x =>
                 x.UpdateRefreshToken(command.Id, It.Is<RefreshToken>(t => t.Token == command.NewToken),
                It.IsAny<CancellationToken>()), Times.Once);

            unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
