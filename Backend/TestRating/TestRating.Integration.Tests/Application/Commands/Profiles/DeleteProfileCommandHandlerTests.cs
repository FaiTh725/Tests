using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestRating.Application.Commands.ProfileEntity.DeleteProfile;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Profiles
{
    [Collection("Integration Tests")]
    public class DeleteProfileCommandHandlerTests : 
        BaseIntegrationTest
    {
        public DeleteProfileCommandHandlerTests(CustomWebFactory factory) : base(factory)
        {}

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteProfileCommand
            {
                Id = 1
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Act
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileExists_ShouldThrow()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var command = new DeleteProfileCommand
            {
                Id = profileFromDb.Entity.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Act
            var deletedProfile = await context.Profiles
                .FirstOrDefaultAsync(x => x.Id == profileFromDb.Entity.Id);

            deletedProfile.Should().BeNull();
        }
    }
}
