using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.ProfileGroupEntity.DeleteGroup;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.ProfileGroups.Commands
{
    public class DeleteGroupCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileGroupRepository> profileGroupRepositoryMock;

        private readonly DeleteGroupHandler handler;

        public DeleteGroupCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileGroupRepositoryMock = new();

            handler = new DeleteGroupHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(profileGroupRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleWhenGroupDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteGroupCommand 
            { 
                GroupId = 1,
                OwnerId = 1,
                Role = "User"
            };

            profileGroupRepositoryMock.Setup(x => x
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
        public async Task HandleWhenGroupExists_ShouldDeleteGroup()
        {
            // Arrange
            var command = new DeleteGroupCommand
            {
                GroupId = 1,
                OwnerId = 1,
                Role = "User"
            };
            var existedGroup = ProfileGroup.Initialize("name", 1).Value;

            profileGroupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            profileGroupRepositoryMock.Verify(x => x
                .DeleteGroup(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
