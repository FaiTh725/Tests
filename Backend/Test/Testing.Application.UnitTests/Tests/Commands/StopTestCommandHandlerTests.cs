using Application.Shared.Exceptions;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using System.Xml.Linq;
using Test.Application.Commands.Test.StopTest;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileAnswerEntity;
using Test.Application.Contracts.Test;
using Test.Application.Contracts.TestSession;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.Tests.Commands
{
    public class StopTestCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestSessionRepository> sessionRepositoryMock;
        private readonly Mock<IProfileAnswerRepository> profileAnswersRepositoryMock;
        private readonly Mock<ITestEvaluatorService> testEvaluatorServiceMock;
        private readonly Mock<ITempDbService<TempTestSession>> tempDbServiceMock;
        private readonly Mock<IBackgroundJobService> backgroundJobServiceMock;

        private readonly StopTestHandler handler;

        public StopTestCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileAnswersRepositoryMock = new();
            sessionRepositoryMock = new();
            testEvaluatorServiceMock = new();
            tempDbServiceMock = new();
            backgroundJobServiceMock = new();

            handler = new StopTestHandler(
                unitOfWorkMock.Object, tempDbServiceMock.Object, 
                testEvaluatorServiceMock.Object, backgroundJobServiceMock.Object);

            unitOfWorkMock.Setup(x => x.SessionRepository)
                .Returns(sessionRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ProfileAnswerRepository)
                .Returns(profileAnswersRepositoryMock.Object);
        }

        [Fact]
        public async Task Handler_WhenSessionDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new StopTestCommand 
            { 
                SessionId = Guid.NewGuid()
            };

            tempDbServiceMock.Setup(x => x
                .GetEntity(
                    command.SessionId))
                .ReturnsAsync(null as TempTestSession);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Session doesnt exist");
        }

        [Fact]
        public async Task Handler_WhenSessionExist_ShouldCalculateResultAndCloseSession()
        {
            // Arrange
            var command = new StopTestCommand
            {
                SessionId = Guid.NewGuid()
            };
            var tempTestSession = new TempTestSession
            {
                Id = Guid.NewGuid(),
                ProfileId = 1,
                Answers = new List<SessionProfileAnswer>(),
                StartTime = DateTime.UtcNow.AddMinutes(-10),
                TestDuration = null,
                TestId = 1,
                JobId = string.Empty
            };
            var addedTestSession = TestSession.Initialize(1, 1).Value;
            // set Id
            var type = typeof(TestSession);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(addedTestSession, [1]);

            tempDbServiceMock.Setup(x => x
                .GetEntity(
                    command.SessionId))
                .ReturnsAsync(tempTestSession);

            sessionRepositoryMock.Setup(x => x
                .AddTestSession(
                    It.IsAny<TestSession>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedTestSession);

            testEvaluatorServiceMock.Setup(x => x
                .Evaluate(
                    It.IsAny<Dictionary<long, SessionProfileAnswer>>(), 
                    It.IsAny<long>(), 
                    It.IsAny<long>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success<TestResult>(new TestResult
                {
                    Percent = 50,
                    ProfileAnswers = new List<ProfileAnswer>(),
                    TestId = 1
                }));

            // Act
            var sessionId = await handler.Handle(command, CancellationToken.None);

            // Assert
            sessionId.Should().Be(addedTestSession.Id);

            unitOfWorkMock.Verify(x => x
                .BeginTransactionAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            unitOfWorkMock.Verify(x => x
                .CommitTransactionAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWorkMock.Verify(x => x
                .RollBackTransactionAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);

            sessionRepositoryMock.Verify(x => x
                .AddTestSession(
                    It.IsAny<TestSession>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            sessionRepositoryMock.Verify(x => x
                .UpdateTestSession(
                    It.IsAny<long>(),
                    It.IsAny<TestSession>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            testEvaluatorServiceMock.Verify(x => x
                .Evaluate(
                     It.IsAny<Dictionary<long, SessionProfileAnswer>>(),
                     It.IsAny<long>(),
                     It.IsAny<long>(),
                     It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
