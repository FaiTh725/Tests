using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.ProfileEntity.AddProfile;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Profiles.Command
{
    public class AddProfileCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;

        private readonly AddProfileHandler handler;

        public AddProfileCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();

            handler = new AddProfileHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileWithRequestEmailExists_ShouldThrowConflictException()
        {
            // Arrange
            var command = new AddProfileCommand
            {
                Email = "zelenukho725@mail.ru",
                Name = "FaITh"
            };
            var profileFromDb = Profile.Initialize("zelenukho725@mail.ru", "Faith2").Value;

            profileRepositoryMock.Setup(x => x
                .GetProfileByEmail(
                    command.Email, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(profileFromDb);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage($"Profile with email {command.Email} " +
                    "already registered");
        }

        [Fact]
        public async Task Handle_WhenProfileWithRequestEmailDoesntExists_ShouldCreateNewProfile()
        {
            // Arrange
            var command = new AddProfileCommand
            {
                Email = "zelenukho725@mail.ru",
                Name = "FaITh"
            };
            var addedProfile = Profile.Initialize("zelenukho725@mail.ru", "FaITh").Value;
            // set profile id instead of db
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(addedProfile, [1]);


            profileRepositoryMock.Setup(x => x
                .GetProfileByEmail(
                    command.Email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            profileRepositoryMock.Setup(x => x
                .AddProfile(
                    It.IsAny<Profile>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedProfile);

            // Act
            var profileId =  await handler.Handle(command, CancellationToken.None);

            // Assert
            profileId.Should().Be(addedProfile.Id);

            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            profileRepositoryMock.Verify(x => x
                .AddProfile(
                    It.IsAny<Profile>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
