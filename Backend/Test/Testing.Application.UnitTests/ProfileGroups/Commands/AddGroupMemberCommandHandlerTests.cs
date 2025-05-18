using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.ProfileGroupEntity.AddGroupMember;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.ProfileGroups.Commands
{
    public class AddGroupMemberCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<IProfileGroupRepository> groupRepositoryMock;

        private readonly AddGroupMemberHandler handler;

        public AddGroupMemberCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();
            groupRepositoryMock = new();

            handler = new AddGroupMemberHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(groupRepositoryMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenGroupDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new AddGroupMemberCommand
            {
                GroupId = 1,
                OwnerId = 1,
                ProfileId = 1,
                Role = "Admin"
            };

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ProfileGroup);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Group doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileAlreadyExistedInGroup_ShouldThrowConflictException()
        {
            // Arrange
            var command = new AddGroupMemberCommand
            {
                GroupId = 1,
                OwnerId = 1,
                ProfileId = 1,
                Role = "Admin"
            };
            var existedGroup = ProfileGroup.Initialize("Name", 2).Value;
            existedGroup.AddMember(1);

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("New member already added");
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldBadRequestException()
        {
            // Arrange
            var command = new AddGroupMemberCommand
            {
                GroupId = 1,
                OwnerId = 1,
                ProfileId = 1,
                Role = "Admin"
            };
            var existedGroup = ProfileGroup.Initialize("Name", 2).Value;

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

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
        public async Task Handle_WhenProfileDoesntExistInGroup_ShouldAddProfileToGroup()
        {
            // Arrange
            var command = new AddGroupMemberCommand
            {
                GroupId = 1,
                OwnerId = 1,
                ProfileId = 1,
                Role = "Admin"
            };
            var existedProfile = Profile.Initialize("Name", "test@mail.com").Value;
            // set id
            var type = typeof(ProfileGroup);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedProfile, [1]);
            var existedGroup = ProfileGroup.Initialize("Name", 2).Value;

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            existedGroup.MembersId.Should().Contain(command.ProfileId);

            groupRepositoryMock.Verify(x => x
                .UpdateGroup(
                    It.IsAny<long>(), 
                    It.IsAny<ProfileGroup>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
