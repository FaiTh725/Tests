using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.ProfileEntity.DeleteProfile;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Profiles.Command
{
    public class DeleteProfileCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
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
        public async Task Handle_WhenProfileDoesntExists_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteProfileCommand
            {
                Id = 1
            };

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage($"Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileWithRequestEmailDoesntExists_ShouldCreateNewProfile()
        {
            // Arrange
            var command = new DeleteProfileCommand
            {
                Id = 1
            };
            var existedProfile = Profile.Initialize("zelenukho725@mail.ru", "FaITh").Value;

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            profileRepositoryMock.Verify(x => x
                .DeleteProfile(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
