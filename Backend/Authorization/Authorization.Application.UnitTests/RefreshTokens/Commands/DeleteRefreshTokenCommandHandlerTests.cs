using Authorization.Application.Commands.RefreshTokenEntity.DeleteRefreshToken;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using Moq;

namespace Authorization.Application.UnitTests.RefreshTokens.Commands
{
    public class DeleteRefreshTokenCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IRefreshTokenRepository> refreshTokenRepositoryMock;
        private readonly DeleteRefreshTokenHandler handler;

        public DeleteRefreshTokenCommandHandlerTests()
        {
            unitOfWorkMock = new();
            refreshTokenRepositoryMock = new();

            handler = new DeleteRefreshTokenHandler(unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_AnyCommand_ShouldExcuteDeleteMethod()
        {
            // Arrange
            var command = new DeleteRefreshTokenCommand
            {
                RefreshToken = "dsdqw2gvs"
            };

            unitOfWorkMock.Setup(x => x
                    .RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            refreshTokenRepositoryMock.Verify(x =>
                x.RemoveToken(
                    command.RefreshToken, 
                    It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWorkMock.Verify(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
