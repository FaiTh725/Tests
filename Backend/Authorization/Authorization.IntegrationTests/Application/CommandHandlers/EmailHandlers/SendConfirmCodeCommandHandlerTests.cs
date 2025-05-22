using Application.Shared.Exceptions;
using Authorization.Application.Commands.Email.SendConfirmCode;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Contracts.User;
using FluentAssertions;
using Notification.Contracts.Email;
using System.Net.Http.Json;

namespace Authorization.IntegrationTests.Application.CommandHandlers.EmailHandlers
{
    public class SendConfirmCodeCommandHandlerTests : BaseIntegrationTest
    {
        public SendConfirmCodeCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenEmailRegistered_ShouldThrowConflictException()
        {
            // Arrange
            var command = new SendConfirmCodeCommand
            {
                Email = "test@mail.com"
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
                .WithMessage("Email already registered");
        }

        [Fact]
        public async Task Handle_WhenEmailIsntRegistered_ShouldSetInCacheEmailAsUnconfirmed()
        {
            // Arrange
            var command = new SendConfirmCodeCommand
            {
                Email = "test@mail.com"
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert

            var userCode = await cache
                .GetData<UserCode>("unconfirmed_mail:" + command.Email);
            userCode.Should().NotBeNull();

            var isPublished = await massTransitHarness.Published.Any<SendEmailRequest>();
            isPublished.Should().BeTrue();
        }
    }
}
