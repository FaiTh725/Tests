using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.ProfileGroupEntity.DeleteMembersGroup;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.ProfileGroups.Commands
{
    public class DeleteMembersGroupCommandHandlerTest
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileGroupRepository> groupRepositoryMock;

        private readonly DeleteMembersGroupHandler handler;

        public DeleteMembersGroupCommandHandlerTest()
        {
            unitOfWorkMock = new();
            groupRepositoryMock = new();

            handler = new DeleteMembersGroupHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(groupRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenGroupDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteMembersGroupCommand
            {
                GroupId = 1,
                MembersId = [1, 2, 3],
                OwnerId = 1,
                Role = "User"
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
        public async Task Handle_WhenMembersInGroup_ShouldDeleteMembersFromGroup()
        {
            // Arrange
            var command = new DeleteMembersGroupCommand
            {
                GroupId = 1,
                MembersId = [1, 2, 3],
                OwnerId = 1,
                Role = "User"
            };
            var existedGroup = ProfileGroup.Initialize("name", 1).Value;
            existedGroup.AddMember(1);
            existedGroup.AddMember(2);
            existedGroup.AddMember(3);
            existedGroup.AddMember(4);

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            existedGroup.MembersId.Should().Contain(4);
            existedGroup.MembersId.Should().HaveCount(1);

            groupRepositoryMock.Verify(x => x
                .UpdateGroup(
                    It.IsAny<long>(), 
                    It.IsAny<ProfileGroup>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
