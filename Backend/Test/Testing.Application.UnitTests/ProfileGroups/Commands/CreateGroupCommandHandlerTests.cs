using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.ProfileGroupEntity.CreateGroup;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.ProfileGroups.Commands
{
    public class CreateGroupCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<IProfileGroupRepository> profileGroupRepositoryMock;

        private readonly CreateGroupHandler handler;

        public CreateGroupCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();
            profileGroupRepositoryMock = new();

            handler = new CreateGroupHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(profileGroupRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadReuquestException()
        {
            // Arrange
            var command = new CreateGroupCommand
            {
                GroupName = "name",
                OwnerId = 1
            };

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.OwnerId, 
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
        public async Task Handle_WhenProfileExist_ShouldAddNewGroup()
        {
            // Arrange
            var command = new CreateGroupCommand
            {
                GroupName = "name",
                OwnerId = 1
            };
            var existedProfile = Profile.Initialize("string123", "test@mail.com").Value;

            var addedGroup = ProfileGroup.Initialize(command.GroupName, 1).Value;
            // set Id
            var type = typeof(ProfileGroup);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(addedGroup, [1]);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.OwnerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            profileGroupRepositoryMock.Setup(x => x
                .AddGroup(
                    It.IsAny<ProfileGroup>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedGroup);

            // Act
            var groupId = await handler.Handle(command, CancellationToken.None);

            // Assert
            groupId.Should().Be(addedGroup.Id);

            profileGroupRepositoryMock.Verify(x => x
                .AddGroup(
                    It.IsAny<ProfileGroup>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
