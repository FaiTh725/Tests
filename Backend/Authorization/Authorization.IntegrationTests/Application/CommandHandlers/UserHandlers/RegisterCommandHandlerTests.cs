using Application.Shared.Exceptions;
using Authorization.Application.Commands.UserEntity.Register;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Authorization.IntegrationTests.Application.CommandHandlers.UserHandlers
{
    public class RegisterCommandHandlerTests : BaseIntegrationTest
    {
        public RegisterCommandHandlerTests(CustomWebFactory factory): 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenUserAlreadyRegistered_ShouldThrowConflictException()
        {
            // Arrange
            var command = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "testpass123",
                UserName = "test"
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
                .ThrowAsync<ConflictException>()
                .WithMessage("User already registered");
        }

        [Theory]
        [InlineData("test@mail.com", "string123", "")]
        [InlineData("test@mail.com", "string", "test")]
        [InlineData("test@mail.com", "12333632", "test")]
        [InlineData("test@mailcom", "string1233", "test")]
        [InlineData("testmail.com", "string724", "test")]
        [InlineData("test@mail.com", "s123", "test")]
        public async Task Handle_WhenCredentialsIsIncorrect_ShouldThrowBadRequestException(string email, string password, string name)
        {
            // Arrange
            var command = new RegisterCommand
            {
                Email = email,
                Password = password,
                UserName = name
            };
            await cache.SetData("confirmed_email:" + email, "Confirmed", 600);

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Handle_WhenCommandIsCorrect_ShouldAddUserAndRefreshToken()
        {
            // Arrange
            var command = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "testpass123",
                UserName = "test"
            };
            await cache.SetData("confirmed_email:" + command.Email, "Confirmed", 600);

            // Act
            var (userId, refreshToken) = await sender.Send(command, CancellationToken.None);

            // Assert
            
            var addedUser = context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
            addedUser.Should().NotBeNull();

            var addedRefreshToken = context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);
            addedRefreshToken.Should().NotBeNull();
        }
    }
}
