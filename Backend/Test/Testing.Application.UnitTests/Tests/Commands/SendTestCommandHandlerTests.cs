using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.Test.SendTestAnswer;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileAnswerEntity;
using Test.Application.Contracts.TestSession;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.Tests.Commands
{
    public class SendTestCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITempDbService<TempTestSession>> tempDbServiceMock;
        private readonly Mock<IQuestionAnswerRepository> questionAnswerRepositoryMock;

        private readonly SendTestAnswerHandler handler;

        public SendTestCommandHandlerTests()
        {
            unitOfWorkMock = new();
            tempDbServiceMock = new();
            questionAnswerRepositoryMock = new();

            handler = new SendTestAnswerHandler(
                unitOfWorkMock.Object, tempDbServiceMock.Object);

            unitOfWorkMock.Setup(x => x.QuestionAnswerRepository)
                .Returns(questionAnswerRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenSessionDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendTestAnswerCommand 
            { 
                QuestionAnswersId = new List<long>(),
                QuestionId = 1,
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
        public async Task Handle_WhenUserSentInvalidAnswers_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendTestAnswerCommand
            {
                QuestionAnswersId = [2],
                QuestionId = 1,
                SessionId = Guid.NewGuid()
            };
            var testSession = new TempTestSession
            {
                Id = Guid.NewGuid(),
                Answers = new List<SessionProfileAnswer>(),
                JobId = string.Empty,
                ProfileId = 1,
                StartTime = DateTime.UtcNow,
                TestDuration = 15,
                TestId = 1
            };
            var existedAnswer = QuestionAnswer.Initialize("some text", true, 1).Value;

            tempDbServiceMock.Setup(x => x
                .GetEntity(
                    command.SessionId))
                .ReturnsAsync(testSession);

            questionAnswerRepositoryMock.Setup(x => x
                .GetQuestionAnswersByCriteria(
                    It.IsAny<AnswersByQuestionIdSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedAnswer]);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Invalid answers id, some answers dont exist");
        }

        [Fact]
        public async Task Handle_WhenUserSentValidAnswers_ShouldAddAnswersToSession()
        {
            // Arrange
            var command = new SendTestAnswerCommand
            {
                QuestionAnswersId = [1],
                QuestionId = 1,
                SessionId = Guid.NewGuid()
            };
            var testSession = new TempTestSession
            {
                Id = Guid.NewGuid(),
                Answers = new List<SessionProfileAnswer>(),
                JobId = string.Empty,
                ProfileId = 1,
                StartTime = DateTime.UtcNow,
                TestDuration = 15,
                TestId = 1
            };
            var existedAnswer = QuestionAnswer.Initialize("some text", true, 1).Value;
            // set Id
            var type = typeof(QuestionAnswer);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedAnswer, [1]);

            tempDbServiceMock.Setup(x => x
                .GetEntity(
                    command.SessionId))
                .ReturnsAsync(testSession);

            questionAnswerRepositoryMock.Setup(x => x
                .GetQuestionAnswersByCriteria(
                    It.IsAny<AnswersByQuestionIdSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedAnswer]);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            tempDbServiceMock.Verify(x => x
                .UpdateEntity(
                    It.IsAny<Guid>(), 
                    It.IsAny<TempTestSession>()), 
                Times.Once);
        }
    }
}
