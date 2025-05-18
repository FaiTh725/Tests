using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.Question.DeleteQuestion;
using Test.Application.Contracts.File;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.TestQuestions.Commands
{
    public class DeleteQuestionCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IQuestionRepository> questionRepositoryMock;
        private readonly Mock<IOutboxService> outboxServiceMock;

        private readonly DeleteQuestionHandler handler;

        public DeleteQuestionCommandHandlerTests()
        {
            unitOfWorkMock = new();
            questionRepositoryMock = new();
            outboxServiceMock = new();

            handler = new DeleteQuestionHandler(unitOfWorkMock.Object, outboxServiceMock.Object);

            unitOfWorkMock.Setup(x => x.QuestionRepository)
                .Returns(questionRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenQuestionDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteQuestionCommand
            {
                QuestionId = 1,
                OwnerId = 1,
                Role = "User"
            };

            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    command.QuestionId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Question);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Question doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenQuestionExists_ShouldDeleteQuestion()
        {
            // Arrange
            var command = new DeleteQuestionCommand
            {
                QuestionId = 1,
                OwnerId = 1,
                Role = "User"
            };
            var existedQuestion = Question.Initialize("question", 5, QuestionType.OneAnswer, 1).Value;

            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    command.QuestionId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedQuestion);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            questionRepositoryMock.Verify(x => x
                .DeleteQuestion(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            unitOfWorkMock.Verify(x => x
                .CommitTransactionAsync(
                    It.IsAny<CancellationToken>()), 
                    Times.Once);

            unitOfWorkMock.Verify(x => x
                .BeginTransactionAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWorkMock.Verify(x => x
                .RollBackTransactionAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);

            outboxServiceMock.Verify(x => x
                .AddOutboxMessage(
                    It.IsAny<DeleteFilesFromStorage>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
