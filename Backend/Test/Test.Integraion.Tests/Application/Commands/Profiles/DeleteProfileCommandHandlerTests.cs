using Application.Shared.Exceptions;
using FluentAssertions;
using MongoDB.Driver;
using Test.Application.Commands.ProfileEntity.DeleteProfile;
using Test.Dal.Persistences;

namespace Test.Integration.Tests.Application.Commands.Profiles
{
    [Collection("Integration Tests")]
    public class DeleteProfileCommandHandlerTests : 
        BaseIntegrationTest
    {
        public DeleteProfileCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBAdRequestException()
        {
            // Arrange
            var command = new DeleteProfileCommand
            {
                ProfileId = 1
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileExistS_ShouldDeleteProfile()
        {
            // Arrange
            var profile = new MongoProfile 
            { 
                Email = "test@mail.com",
                Id = 1,
                Name = "test" 
            };

            await context.Profiles.InsertOneAsync(profile);

            var command = new DeleteProfileCommand
            {
                ProfileId = profile.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert
            var deletedProfile = await context.Profiles
                .Find(x => x.Id == command.ProfileId)
                .FirstOrDefaultAsync();

            deletedProfile.Should().BeNull();
        }
    }
}
