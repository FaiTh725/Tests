using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.ProfileEntity.CreateProfile;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.Profiles.Commands
{
    public class CreateProfileCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;

        private readonly CreateProfileHandler handler;

        public CreateProfileCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();

            handler = new CreateProfileHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileExists_ShouldThrowConflictException()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                Name = "test",
                Email = "test@mail.ru"
            };
            var existedProfile = Profile.Initialize("test1", "test@mail.ru").Value;

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.Email, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            var act = async () => await handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Email " + command.Email +
                    " has already registered");
        }

        [Fact]
        public async Task Handle_WhenProfileWithCommandEmailIsntRegistered_ShouldCreateProfile()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                Name = "test",
                Email = "test@mail.ru"
            };
            var addedProfile = Profile.Initialize("test1", "test@mail.ru").Value;
            // set Id instead of db
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(addedProfile, [1]);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.Email,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            profileRepositoryMock.Setup(x => x
                .AddProfile(
                    It.IsAny<Profile>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedProfile);

            // Act
            var profileId = await handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            profileId.Should().Be(addedProfile.Id);

            profileRepositoryMock.Verify(x => x
                .AddProfile(
                    It.IsAny<Profile>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
