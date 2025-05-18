using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.ProfileEntity.DeleteProfile;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.Profiles.Commands
{
    public class DeleteProfileCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;

        private readonly DeleteProfileHandler handler;

        public DeleteProfileCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();

            handler = new DeleteProfileHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteProfileCommand
            {
                ProfileId = 1
            };

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.ProfileId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileExist_ShouldDeleteProfile()
        {
            // Arrange
            var command = new DeleteProfileCommand
            {
                ProfileId = 1
            };
            var existedProfile = Profile.Initialize("test", "test@mail.ru").Value;

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            profileRepositoryMock.Verify(x => x
                .DeleteProfile(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
