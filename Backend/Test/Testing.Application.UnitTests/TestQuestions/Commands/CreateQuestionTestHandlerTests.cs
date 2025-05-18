using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.Question.CreateQuestion;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.TestQuestions.Commands
{
    public class CreateQuestionTestHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestRepository> testRepositoryMock;
        private readonly Mock<IQuestionRepository> questionRepositoryMock;
        private readonly Mock<IQuestionAnswerRepository> answerRepositoryMock;
        private readonly Mock<IBlobService> blobServiceMock;

        private readonly CreateQuestionHandler handler;

        public CreateQuestionTestHandlerTests()
        {
            unitOfWorkMock = new();
            testRepositoryMock = new();
            questionRepositoryMock = new();
            answerRepositoryMock = new();
            blobServiceMock = new();

            handler = new CreateQuestionHandler(
                unitOfWorkMock.Object, blobServiceMock.Object);

            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.QuestionRepository)
                .Returns(questionRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.QuestionAnswerRepository)
                .Returns(answerRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTaskDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new CreateQuestionCommand
            {
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 5,
                TestId = 1,
                TestQuestion = "text here",
                OwnerId = 1,
                Role = "User",
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
        public async Task Handle_WhenQuestionWithManyAnswersDoesntHaveAnyAnswers_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new CreateQuestionCommand
            {
                QuestionType = QuestionType.ManyAnswers,
                QuestionWeight = 5,
                TestId = 1,
                TestQuestion = "text here",
                OwnerId = 1,
                Role = "User",
            };
            var test = TestEntity.Initialize("name", "description", 1, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(test);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Answer should have 1 correct answer if the type is OneAnswer " +
                    "or at least one answer");
        }

        [Fact]
        public async Task Handle_WhenQuestionWithOneAnswerHasManyCorrectAnswers_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new CreateQuestionCommand
            {
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 5,
                TestId = 1,
                TestQuestion = "text here",
                OwnerId = 1,
                Role = "User",
                Answers = new List<CreateQuestionAnswer>
                {
                    new CreateQuestionAnswer
                    {
                        Answer = "test",
                        IsCorrect = true,
                    },
                    new CreateQuestionAnswer
                    {
                        Answer = "test",
                        IsCorrect = true,
                    },
                }
            };
            var test = TestEntity.Initialize("name", "description", 1, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(test);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Answer should have 1 correct answer if the type is OneAnswer " +
                    "or at least one answer");
        }

        [Fact]
        public async Task Handle_WhenCommandIsCorrect_ShouldAddTestQuestion()
        {
            // Arrange
            var command = new CreateQuestionCommand
            {
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 5,
                TestId = 1,
                TestQuestion = "text here",
                OwnerId = 1,
                Role = "User",
                Answers = new List<CreateQuestionAnswer>
                {
                    new CreateQuestionAnswer
                    {
                        Answer = "test",
                        IsCorrect = true,
                    }
                }
            };
            var test = TestEntity.Initialize("name", "description", 1, TestType.Timed, 15, true).Value;
            var addedQuestion = Question.Initialize("Test", 12, QuestionType.OneAnswer, 1).Value;
            // set Id
            var type = typeof(Question);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(addedQuestion, [1]);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(test);

            questionRepositoryMock.Setup(x => x
                .AddQuestion(
                    It.IsAny<Question>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedQuestion);
            
            // Act
            var questionId = await handler.Handle(command, CancellationToken.None);

            // Assert
            questionId.Should().Be(addedQuestion.Id);

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

            questionRepositoryMock.Verify(x => x
                .AddQuestion(
                    It.IsAny<Question>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
