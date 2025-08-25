using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestRating.Application.Commands.ProfileEntity.AddProfile;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Profiles
{
    [Collection("Integration Tests")]
    public class AddProfileCommandHandlerTests :
        BaseIntegrationTest
    {
        public AddProfileCommandHandlerTests(CustomWebFactory factory) :
            base(factory)
        { }

        [Fact]
        public async Task Handle_WhenProfileAlreadyRegistered_ShouldThrowConflictException()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var command = new AddProfileCommand
            {
                Email = "test@mail.com",
                Name = "test"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage($"Profile with email {command.Email} " +
                    "already registered");
        }

        [Theory]
        [InlineData("test@mailcom", "name")]
        [InlineData("test@mail.com", "")]
        [InlineData("testmail.com", "name")]
        public async Task Handle_WhenCommandIsIncorrect_ShouldThrowBadRequestException(string email, string name)
        {
            // Arrange
            var command = new AddProfileCommand
            {
                Email = email,
                Name = name
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Handle_WhenCommandIsCorrect_ShouldAddProfile()
        {
            // Arrange
            var command = new AddProfileCommand
            {
                Email = "test@mail.com",
                Name = "test"
            };

            // Act
            var profileId = await sender.Send(command, CancellationToken.None);

            // Assert
            var profile = await context.Profiles
                .FirstOrDefaultAsync(x => x.Id == profileId);

            profile.Should().NotBeNull();
            profile.Email.Should().Be(command.Email);
            profile.Name.Should().Be(command.Name);
        }
    }
}
