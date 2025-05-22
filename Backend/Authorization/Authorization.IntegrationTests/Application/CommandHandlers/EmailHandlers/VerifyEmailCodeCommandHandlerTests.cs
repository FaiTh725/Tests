using Application.Shared.Exceptions;
using Authorization.Application.Commands.Email.VerifyCode;
using Authorization.Application.Contracts.User;
using FluentAssertions;

namespace Authorization.IntegrationTests.Application.CommandHandlers.EmailHandlers
{
    public class VerifyEmailCodeCommandHandlerTests : 
        BaseIntegrationTest
    {
        public VerifyEmailCodeCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenInvalidCodeOrEmail_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new VerifyCodeCommand
            {
                Email = "test@mail.com",
                Code = "123"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Invalid code or email");
        }

        [Fact]
        public async Task Handle_WhenCodeIsValid_ShouldRemoveOldCacheAndSetEmailAsConfirmed()
        {
            // Arrange
            var command = new VerifyCodeCommand
            {
                Email = "test@mail.com",
                Code = "123"
            };

            var userCode = new UserCode()
            {
                Code = "123",
                SendingTime = DateTime.UtcNow.AddMinutes(-10)
            };
            await cache.SetData("unconfirmed_mail:" + command.Email, userCode, 60);

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert
            var oldCache = await cache
                .GetData<UserCode>("unconfirmed_mail:" + command.Email);
            oldCache.IsFailure.Should().BeTrue();

            var newCache = await cache
                .GetData<string>("confirmed_email:" + command.Email);
            newCache.Should().NotBeNull();
        }
    }
}
