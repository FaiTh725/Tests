using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.Question.UpdateQuestion;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.TestQuestions.Commands
{
    public class UpdateQuestionCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IQuestionRepository> questionRepositoryMock;

        private readonly UpdateQuestionHandler handler;

        public UpdateQuestionCommandHandlerTests()
        {
            unitOfWorkMock = new();
            questionRepositoryMock = new();

            handler = new UpdateQuestionHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.QuestionRepository)
                .Returns(questionRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenQuestionDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new UpdateQuestionCommand
            {
                QuestionId = 1,
                QuestionWeight = 8,
                TestQuestion = "new text",
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
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Question doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenQuestionExist_ShouldUpdateQuestion()
        {
            // Arrange
            var command = new UpdateQuestionCommand
            {
                QuestionId = 1,
                QuestionWeight = 8,
                TestQuestion = "new text",
                OwnerId = 1,
                Role = "User"
            };
            var existedQuestion = Question.Initialize("old text", 3, QuestionType.OneAnswer, 1).Value;
            // set Id
            var type = typeof(Question);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedQuestion, [1]);

            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    command.QuestionId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedQuestion);

            // Act
            var updatedQuestionId = await handler.Handle(command, CancellationToken.None);

            // Assert
            updatedQuestionId.Should().Be(existedQuestion.Id);
            existedQuestion.TestQuestion.Should().Be(command.TestQuestion);
            existedQuestion.QuestionWeight.Should().Be(command.QuestionWeight);

            questionRepositoryMock.Verify(x => x
                .UpdateQuestion(
                    It.IsAny<long>(), 
                    It.IsAny<Question>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
