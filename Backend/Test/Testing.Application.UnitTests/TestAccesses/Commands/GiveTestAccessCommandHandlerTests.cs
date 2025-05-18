using Moq;
using Test.Application.Commands.TestAccessEntity.GiveAccessTest;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using TestEntity = Test.Domain.Entities.Test;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using FluentAssertions;
using Application.Shared.Exceptions;
using Test.Domain.Entities;

namespace Testing.Application.UnitTests.TestAccesses.Commands
{
    public class GiveTestAccessCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestRepository> testRepositoryMock;
        private readonly Mock<ITestAccessRepository> testAccessRepositoryMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<IProfileGroupRepository> groupRepositoryMock;

        private readonly GiveAccessTestHandler handler;

        public GiveTestAccessCommandHandlerTests()
        {
            unitOfWorkMock = new();
            testRepositoryMock = new();
            testAccessRepositoryMock = new();
            profileRepositoryMock = new();
            groupRepositoryMock = new();

            handler = new GiveAccessTestHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(groupRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AccessRepository)
                .Returns(testAccessRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new GiveAccessTestCommand
            {
                AccessTargetEntityId = 1,
                TargetEntity = TargetAccessEntityType.Profile,
                TestId = 1,
                OwnerId = 1,
                Role = "User"
            };

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestEntity);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Test doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTestAccessAlreadyExist_ShouldThrowConflictException()
        {
            // Arrange
            var command = new GiveAccessTestCommand
            {
                AccessTargetEntityId = 1,
                TargetEntity = TargetAccessEntityType.Profile,
                TestId = 1,
                OwnerId = 1,
                Role = "User"
            };
            var existedTest = TestEntity.Initialize("test", "description", 1, TestType.Timed, 15, true).Value;
            var existedTestAccess = TestAccess.Initialize(1, 1, TargetAccessEntityType.Profile).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            testAccessRepositoryMock.Setup(x => x
                .GetTestAccess(
                    command.TestId, 
                    command.AccessTargetEntityId, 
                    command.TargetEntity, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTestAccess);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Entity already has access");
        }

        [Fact]
        public async Task Handle_WhenTargetEntityProfileAndProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new GiveAccessTestCommand
            {
                AccessTargetEntityId = 1,
                TargetEntity = TargetAccessEntityType.Profile,
                TestId = 1,
                OwnerId = 1,
                Role = "User"
            };
            var existedTest = TestEntity.Initialize("test", "description", 1, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            testAccessRepositoryMock.Setup(x => x
                .GetTestAccess(
                    command.TestId,
                    command.AccessTargetEntityId,
                    command.TargetEntity,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestAccess);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.AccessTargetEntityId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Target entity doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTargetEntityGroupAndGroupDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new GiveAccessTestCommand
            {
                AccessTargetEntityId = 1,
                TargetEntity = TargetAccessEntityType.Group,
                TestId = 1,
                OwnerId = 1,
                Role = "User"
            };
            var existedTest = TestEntity.Initialize("test", "description", 1, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            testAccessRepositoryMock.Setup(x => x
                .GetTestAccess(
                    command.TestId,
                    command.AccessTargetEntityId,
                    command.TargetEntity,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestAccess);

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.AccessTargetEntityId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ProfileGroup);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Target entity doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTargetEntityExists_ShouldGiveAccess()
        {
            // Arrange
            var command = new GiveAccessTestCommand
            {
                AccessTargetEntityId = 1,
                TargetEntity = TargetAccessEntityType.Group,
                TestId = 1,
                OwnerId = 1,
                Role = "User"
            };
            var existedTest = TestEntity.Initialize("test", "description", 1, TestType.Timed, 15, true).Value;
            var existedGroup = ProfileGroup.Initialize("group", 1).Value;
            var addedTestAccess = TestAccess.Initialize(1, 1, TargetAccessEntityType.Group).Value;
            // set Id
            var type = typeof(TestAccess);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(addedTestAccess, [1]);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            testAccessRepositoryMock.Setup(x => x
                .GetTestAccess(
                    command.TestId,
                    command.AccessTargetEntityId,
                    command.TargetEntity,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestAccess);

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    command.AccessTargetEntityId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            testAccessRepositoryMock.Setup(x => x
                .AddTestAccess(
                    It.IsAny<TestAccess>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedTestAccess);

            // Act
            var testAccessId = await handler.Handle(command, CancellationToken.None);

            // Assert
            testAccessId.Should().Be(addedTestAccess.Id);

            testAccessRepositoryMock.Verify(x => x
                .AddTestAccess(
                    It.IsAny<TestAccess>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
