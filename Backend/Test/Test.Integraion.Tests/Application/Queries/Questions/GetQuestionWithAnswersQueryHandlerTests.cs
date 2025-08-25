using Application.Shared.Exceptions;
using FluentAssertions;
using Test.Application.Contracts.Question;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Application.Queries.QuestionEntity.GetQuestionWithAnswers;
using Test.Dal.Persistences;
using Test.Domain.Enums;

namespace Test.Integration.Tests.Application.Queries.Questions
{
    [Collection("Integration Tests")]
    public class GetQuestionWithAnswersQueryHandlerTests :
        BaseIntegrationTest
    {
        public GetQuestionWithAnswersQueryHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenQuestionDoesntExist_ShouldThrowsNotFoundException()
        {
            // Arrange
            var query = new GetQuestionWithAnswersQuery
            {
                Id = 1
            };

            // Act
            var act = async () => await sender.Send(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Question doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenQuestionExists_ShouldReturnsQuestionWithAnswers()
        {
            // Arrange
            var question = new MongoQuestion
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestId = 1,
                TestQuestion = "question??"
            };
            var questionAnswer = new MongoQuestionAnswer
            {
                Id = 1,
                Answer = "it is true",
                IsCorrect = true,
                QuestionId = question.Id
            };

            var insertTasks = new List<Task>()
            {
                context.Answers.InsertOneAsync(questionAnswer),
                context.Questions.InsertOneAsync(question)
            };

            await Task.WhenAll(insertTasks);

            var query = new GetQuestionWithAnswersQuery
            {
                Id = 1
            };

            var expectedResult = new QuestionWithAnswersResponse
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestQuestion = "question??",
                Answers = new List<QuestionAnswerResponse>
                {
                    new QuestionAnswerResponse
                    {
                        Id = 1,
                        Answer = "it is true",
                        IsCorrect = true,
                        QuestionId = 1
                    }
                }
            };

            // Act
            var questionWithAnswers =  await sender.Send(query, CancellationToken.None);

            // Assert
            questionWithAnswers.Should().BeEquivalentTo(expectedResult);
        }
    }
}
