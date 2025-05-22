using Authorization.Application.Commands.RefreshTokenEntity.DeleteRefreshToken;
using Authorization.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Authorization.IntegrationTests.Application.CommandHandlers.RefreshTokenHandlers
{
    public class DeleteRefreshTokenCommandHandlerTests : BaseIntegrationTest
    {
        public DeleteRefreshTokenCommandHandlerTests(
            CustomWebFactory factory) : base(factory)
        {}

        [Fact]
        public async Task Handle_ShouldRemoveToken()
        {
            // Arrange
            var command = new DeleteRefreshTokenCommand
            {
                RefreshToken = "a214g4325"
            };

            var userEntity = User.Initialize("test", "test@mail.com", "grd235yrewef", "User").Value;
            var user = await context.Users.AddAsync(userEntity);

            var refreshToken = RefreshToken.Initialize(
                command.RefreshToken, user.Entity, DateTime.UtcNow.AddDays(15)).Value;
            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert
            var refreshTokenFromDb = await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == command.RefreshToken);

            refreshTokenFromDb.Should().BeNull();
        }
    }
}
