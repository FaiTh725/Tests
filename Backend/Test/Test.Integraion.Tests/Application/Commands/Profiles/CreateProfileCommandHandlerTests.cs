using Application.Shared.Exceptions;
using FluentAssertions;
using MongoDB.Driver;
using Test.Application.Commands.ProfileEntity.CreateProfile;
using Test.Contracts.Profile;
using Test.Dal.Persistences;

namespace Test.Integration.Tests.Application.Commands.Profiles
{
    [Collection("Integration Tests")]
    public class CreateProfileCommandHandlerTests : 
        BaseIntegrationTest
    {
        public CreateProfileCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenProfileWithRequestEmailRegistered_ShouldThrowConflictException()
        {
            // Arrange
            var existedProfile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "name",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(existedProfile);
            
            var command = new CreateProfileCommand
            {
                Email = "test@mail.com",
                Name = "name"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Email " + command.Email +
                    " has already registered");
        }

        [Theory]
        [InlineData("", "name")]
        [InlineData("email.com", "name")]
        [InlineData("test@com", "name")]
        [InlineData("test@.com", "")]
        public async Task Handle_WhenCommandIsIncorrect_ShouldThrowBadRequestException(
            string email, string name)
        {
            // Arrange
            var command = new CreateProfileCommand
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
        public async Task Handle_WhenCommandIsCorrect_ShouldAddNewProfile()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                Email = "test@mail.com",
                Name = "name"
            };

            // Act
            var profileId = await sender.Send(command, CancellationToken.None);

            // Assert
            var addedProfile = await context.Profiles
                .Find(x => x.Id == profileId)
                .FirstOrDefaultAsync();

            addedProfile.Should().NotBeNull();
            addedProfile.Email.Should().Be("test@mail.com");
            addedProfile.Name.Should().Be("name");
        }
    }
}
