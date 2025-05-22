using Application.Shared.Exceptions;
using Authorization.Application.Commands.RefreshTokenEntity.RefreshRefreshToken;
using Authorization.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Authorization.IntegrationTests.Application.CommandHandlers.RefreshTokenHandlers
{
    public class RefreshRefreshTokenCommandHandlersTests : BaseIntegrationTest
    {
        public RefreshRefreshTokenCommandHandlersTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenTokenDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new RefreshRefreshTokenCommand
            { 
                Id = 1,
                NewToken = "fgewr1234twe"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Token doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTokenExistS_ShouldUpdateOldRefreshToken()
        {
            // Arrange
            var userEntity = User.Initialize("test", "test@mail.com", "grd235yrewef", "User").Value;
            var user = await context.Users.AddAsync(userEntity);

            var refreshToken = RefreshToken.Initialize(
                "fgwet235", user.Entity, DateTime.UtcNow.AddDays(1)).Value;
            var existedRefreshToken = await context.RefreshTokens.AddAsync(refreshToken);

            await context.SaveChangesAsync();

            var command = new RefreshRefreshTokenCommand
            {
                Id = existedRefreshToken.Entity.Id,
                NewToken = "fgsdfw23trewtqwfdf"
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert

            var updatedRefreshToken = await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Id == existedRefreshToken.Entity.Id);

            updatedRefreshToken.Should().NotBeNull();
            updatedRefreshToken.ExpireOn.Should()
                .NotBe(DateTime.UtcNow.AddDays(1));
            updatedRefreshToken.Token.Should().Be(command.NewToken);
        }
    }
}
