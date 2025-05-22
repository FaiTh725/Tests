using Application.Shared.Exceptions;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Authorization.IntegrationTests.Application.CommandHandlers.UserHandlers
{
    public class LoginCommandHandlerTests : 
        BaseIntegrationTest
    {
        public LoginCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenEmailIsntRegistered_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@mail.com",
                Password = "test324"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage($"Email {command.Email} isnt registered");
        }

        [Fact]
        public async Task Handle_WhenInvalidCredentials_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@mail.com",
                Password = "test324"
            };
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string321",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + requestToRegister.Email, "Confirmed", 600);
            await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage($"Email invalid password or email");
        }

        [Fact]
        public async Task Handle_WhenValidCommand_ShouldReturnsUserIdAndRefreshToken()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@mail.com",
                Password = "test324"
            };
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "test324",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + requestToRegister.Email, "Confirmed", 600);
            await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            // Act
            var (userId, refreshToken) = await sender.Send(command, CancellationToken.None);

            // Assert
            var existedUser = context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
            existedUser.Should().NotBeNull();

            var existedRefreshToken = context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);
            existedRefreshToken.Should().NotBeNull();
        }
    }
}
