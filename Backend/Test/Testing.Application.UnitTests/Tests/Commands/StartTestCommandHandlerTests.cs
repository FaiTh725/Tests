using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Test.Application.Commands.Test.StartTest;
using Test.Application.Common.Interfaces;
using Test.Application.Common.Wrappers;
using Test.Application.Contracts.TestSession;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Commands
{
    public class StartTestCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestRepository> testRepositoryMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<ITempDbService<TempTestSession>> tempDbServiceMock;
        private readonly Mock<IBackgroundJobService> backgorundJobServiceMock;

        private readonly StartTestHandler handler;

        public StartTestCommandHandlerTests()
        {
            unitOfWorkMock = new();
            testRepositoryMock = new();
            profileRepositoryMock = new();
            tempDbServiceMock = new();
            backgorundJobServiceMock = new();

            handler = new StartTestHandler(
                unitOfWorkMock.Object, 
                tempDbServiceMock.Object, 
                backgorundJobServiceMock.Object);

            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new StartTestCommand
            {
                TestId = 1,
                ProfileId = 1
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
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new StartTestCommand
            {
                TestId = 1,
                ProfileId = 1
            };
            var existedTest = TestEntity.Initialize("name", "description", 1, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

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
        public async Task Handle_WhenStartTimedTest_ShouldCreateSessionAndDelayedJob()
        {
            // Arrange
            var command = new StartTestCommand
            {
                TestId = 1,
                ProfileId = 1
            };
            var existedTest = TestEntity.Initialize("name", "description", 1, TestType.Timed, 15, true).Value;
            var existedProfile = Profile.Initialize("test", "test@mail.ru").Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            backgorundJobServiceMock.Verify(x => x
                .CreateDelayedJob(
                    It.IsAny<Expression<Func<MediatorWrapper, Task>>>(), 
                    It.IsAny<TimeSpan>()), 
                Times.Once);

            tempDbServiceMock.Verify(x => x
                .AddEntity(
                    It.IsAny<TempTestSession>()), 
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenStartProgressiveTest_ShouldCreateSession()
        {
            // Arrange
            var command = new StartTestCommand
            {
                TestId = 1,
                ProfileId = 1
            };
            var existedTest = TestEntity.Initialize("name", "description", 1, TestType.Progressive, null).Value;
            var existedProfile = Profile.Initialize("test", "test@mail.ru").Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            backgorundJobServiceMock.Verify(x => x
                .CreateDelayedJob(
                    It.IsAny<Expression<Action<MediatorWrapper>>>(),
                    It.IsAny<TimeSpan>()),
                Times.Never);

            tempDbServiceMock.Verify(x => x
                .AddEntity(
                    It.IsAny<TempTestSession>()),
                Times.Once);
        }
    }
}
