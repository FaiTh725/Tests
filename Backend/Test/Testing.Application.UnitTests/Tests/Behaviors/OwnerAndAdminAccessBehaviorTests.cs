using Application.Shared.Exceptions;
using FluentAssertions;
using MediatR;
using Moq;
using Test.Application.Behaviors;
using Test.Application.Commands.Test.DeleteTest;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Behaviors
{
    public class OwnerAndAdminAccessBehaviorTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestRepository> testRepositoryMock;

        private readonly OwnerAndAdminTestAccessBehavior<DeleteTestCommand, string> behavior;
        private readonly Mock<RequestHandlerDelegate<string>> nextMock;

        public OwnerAndAdminAccessBehaviorTests()
        {
            unitOfWorkMock = new();
            testRepositoryMock = new();

            behavior = new OwnerAndAdminTestAccessBehavior<DeleteTestCommand, string>(unitOfWorkMock.Object);
            nextMock = new();

            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteTestCommand()
            { 
                OwnerId = 1,
                Role = "User",
                TestId = 1
            };

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestEntity);

            // Act
            var act = async () => await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Test doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileIsntOwner_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var command = new DeleteTestCommand()
            {
                OwnerId = 1,
                Role = "User",
                TestId = 1
            };
            var existedTest = TestEntity.Initialize("name", "description", 2, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            // Act
            var act = async () => await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ForbiddenAccessException>()
                .WithMessage("Only the owner or an admin have access to the test");
        }

        [Fact]
        public async Task Handle_WhenUserIsAdmin_ShouldReturnNextResult()
        {
            // Arrange
            var expectedResult = "something";

            var command = new DeleteTestCommand()
            {
                OwnerId = 1,
                Role = "Admin",
                TestId = 1
            };
            var existedTest = TestEntity.Initialize("name", "description", 2, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            nextMock.Setup(x => x
                .Invoke(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("something");

            // Act
            var result = await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public async Task Handle_WhenProfileIsOwner_ShouldReturnNextResult()
        {
            // Arrange
            var expectedResult = "something";

            var command = new DeleteTestCommand()
            {
                OwnerId = 1,
                Role = "User",
                TestId = 1
            };
            var existedTest = TestEntity.Initialize("name", "description", 1, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            nextMock.Setup(x => x
                .Invoke(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("something");

            // Act
            var result = await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
