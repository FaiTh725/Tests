using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.Question;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Application.Queries.QuestionEntity.GetQuestionWithAnswers;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.TestQuestions.Queries
{
    public class GetQuestionWithAnswersQueryHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IQuestionRepository> questionRepositoryMock;
        private readonly Mock<IQuestionAnswerRepository> answerRepositoryMock;
        private readonly Mock<IBlobService> blobServiceMock;

        private readonly GetQuestionWithAnswersHandler handler;

        public GetQuestionWithAnswersQueryHandlerTests()
        {
            unitOfWorkMock = new();
            questionRepositoryMock = new();
            answerRepositoryMock = new();
            blobServiceMock = new();

            handler = new GetQuestionWithAnswersHandler(unitOfWorkMock.Object, blobServiceMock.Object);

            unitOfWorkMock.Setup(x => x.QuestionRepository)
                .Returns(questionRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.QuestionAnswerRepository)
                .Returns(answerRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenQuestionDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetQuestionWithAnswersQuery 
            { 
                Id = 1
            };

            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Question);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Question doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenQuestionExists_ShouldReturnQuestionWithAnswers()
        {
            // Arrange
            var expectedResult = new QuestionWithAnswersResponse
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 5,
                TestQuestion = "text",
                Answers = new List<QuestionAnswerResponse>
                {
                    new QuestionAnswerResponse
                    {
                        Id = 1,
                        Answer = "answer is correct!!",
                        IsCorrect = true,
                        QuestionId = 1
                    }
                }
            };

            var query = new GetQuestionWithAnswersQuery
            {
                Id = 1
            };
            var existedQuestion = Question.Initialize("text", 5, QuestionType.OneAnswer, 1).Value;
            // set Id
            var type = typeof(Question);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedQuestion, [1]);

            var existedAnswer = QuestionAnswer.Initialize("answer is correct!!", true, 1).Value;
            // set Id
            type = typeof(QuestionAnswer);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedAnswer, [1]);

            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    query.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedQuestion);

            answerRepositoryMock.Setup(x => x
                .GetQuestionAnswersByCriteria(
                    It.IsAny<AnswersByQuestionIdSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedAnswer]);

            // Act
            var questionWithAnswers = await handler.Handle(query, CancellationToken.None);

            // Assert
            questionWithAnswers.Should().BeEquivalentTo(expectedResult);
        }
    }
}
