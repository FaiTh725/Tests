using Application.Shared.Exceptions;
using FluentAssertions;
using MediatR;
using Moq;
using Test.Application.Behaviors;
using Test.Application.Commands.ProfileGroupEntity.DeleteGroup;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using static System.Net.Mime.MediaTypeNames;

namespace Testing.Application.UnitTests.ProfileGroups.Behaviors
{
    public class OwnerAndAdminAccessBehaviorTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileGroupRepository> groupRepositoryMock;

        private readonly OwnerAndAdminGroupAccessBehavior<DeleteGroupCommand, string> behavior;
        private readonly Mock<RequestHandlerDelegate<string>> nextMock;

        public OwnerAndAdminAccessBehaviorTests()
        {
            unitOfWorkMock = new();
            groupRepositoryMock = new();

            behavior = new OwnerAndAdminGroupAccessBehavior<DeleteGroupCommand, string>(unitOfWorkMock.Object);
            nextMock = new();

            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(groupRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenGroupDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteGroupCommand()
            {
                GroupId = 1,
                OwnerId = 1,
                Role = "User"
            };

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ProfileGroup);

            // Act
            var act = async () => await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Group doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileIsNotOwner_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var command = new DeleteGroupCommand()
            {
                GroupId = 1,
                OwnerId = 1,
                Role = "User"
            };

            var existedGroup = ProfileGroup.Initialize("Name", 2).Value;

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            // Act
            var act = async () => await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ForbiddenAccessException>()
                .WithMessage("Only the owner or an admin have access to the group");
        }

        [Fact]
        public async Task Handle_WhenUserIsAdmin_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var expectedResult = "Something";

            var command = new DeleteGroupCommand()
            {
                GroupId = 1,
                OwnerId = 1,
                Role = "Admin"
            };

            var existedGroup = ProfileGroup.Initialize("Name", 2).Value;

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            nextMock.Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");

            // Act
            var result = await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public async Task Handle_WhenUserIsOwner_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var expectedResult = "Something";

            var command = new DeleteGroupCommand()
            {
                GroupId = 1,
                OwnerId = 1,
                Role = "Admin"
            };

            var existedGroup = ProfileGroup.Initialize("Name", 1).Value;

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.GroupId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            nextMock.Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");

            // Act
            var result = await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
