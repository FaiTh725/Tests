using Application.Shared.Exceptions;
using FluentAssertions;
using MediatR;
using Moq;
using Test.Application.Behaviors;
using Test.Application.Commands.Test.StartTest;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Application.Queries.TestAccessEntity.Specifications;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Behaviors
{
    public class TestAccessBehaviorTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileGroupRepository> groupRepositoryMock;
        private readonly Mock<ITestAccessRepository> testAccessRepositoryMock;
        private readonly Mock<ITestRepository> testRepositoryMock;

        private readonly TestAccessBehavior<StartTestCommand, string> behavior;
        private readonly Mock<RequestHandlerDelegate<string>> nextMock;

        public TestAccessBehaviorTests()
        {
            unitOfWorkMock = new();
            groupRepositoryMock = new();
            testAccessRepositoryMock = new();
            testRepositoryMock = new();
            testRepositoryMock = new();

            behavior = new TestAccessBehavior<StartTestCommand, string>(unitOfWorkMock.Object);
            nextMock = new();

            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(groupRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AccessRepository)
                .Returns(testAccessRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestIsPublic_ShouldExecuteNext()
        {
            // Arrange
            var expectedResult = "something";

            var command = new StartTestCommand()
            {
                ProfileId = 1,
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
        public async Task Handle_WhenTestDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new StartTestCommand()
            {
                ProfileId = 1,
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
        public async Task Handle_WhenProfileInOpenAccessGroup_ShouldExecuteNext()
        {
            // Arrange
            var expectedResult = "something";

            var command = new StartTestCommand()
            {
                ProfileId = 1,
                TestId = 1
            };
            var existedTest = TestEntity.Initialize("name", "description", 2, TestType.Timed, 15, false).Value;
            var existedTestAccess = TestAccess.Initialize(1, 1, TargetAccessEntityType.Group).Value;
            var existedGroup = ProfileGroup.Initialize("name", 1).Value;
            existedGroup.AddMember(1);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            groupRepositoryMock.Setup(x => x
                .GetProfileGroupsByCriteria(
                    It.IsAny<GroupsByProfileIdSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedGroup]);

            testAccessRepositoryMock.Setup(x => x
                .GetAccessesByCriteria(
                    It.IsAny<AccessesByTargetEntitySpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedTestAccess]);

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
        public async Task Handle_WhenProfileDoesntHaveAccess_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var command = new StartTestCommand()
            {
                ProfileId = 1,
                TestId = 1
            };
            var existedTest = TestEntity.Initialize("name", "description", 2, TestType.Timed, 15, false).Value;
            var existedGroup = ProfileGroup.Initialize("name", 1).Value;
            existedGroup.AddMember(1);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            groupRepositoryMock.Setup(x => x
                .GetProfileGroupsByCriteria(
                    It.IsAny<GroupsByProfileIdSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedGroup]);

            testAccessRepositoryMock.Setup(x => x
                .GetAccessesByCriteria(
                    It.IsAny<AccessesByTargetEntitySpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            nextMock.Setup(x => x
                .Invoke(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("something");

            // Act
            var act = async () => await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ForbiddenAccessException>()
                .WithMessage("Current test is private");
        }
    }
}
