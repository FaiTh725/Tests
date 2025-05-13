using Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
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
            var command = new RefreshRefreshTokenCommand { Id = 1, NewToken = "" }; 

            // Act

            // Assert
        }
    }
}
