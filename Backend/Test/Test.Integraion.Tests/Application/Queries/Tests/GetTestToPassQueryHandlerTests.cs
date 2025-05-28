using Application.Shared.Exceptions;
using FluentAssertions;
using Test.Application.Contracts.Question;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.Test.GetTestToPass;
using Test.Dal.Persistences;
using Test.Domain.Enums;

namespace Test.Integration.Tests.Application.Queries.Tests
{
    [Collection("Integration Tests")]
    public class GetTestToPassQueryHandlerTests : BaseIntegrationTest
    {
        public GetTestToPassQueryHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetTestToPassQuery
            {
                Id = 1
            };

            // Act
            var act = async () => await sender.Send(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Test doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTestExistS_ShouldReturnsTestToPass()
        {
            // Arrange
            var test = new MongoTest
            {
                Id = 1,
                CreatedTime = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                DurationInMinutes = 15,
                Description = "description",
                IsPublic = true,
                Name = "test name",
                ProfileId = 1,
                TestType = TestType.Timed
            };
            var question = new MongoQuestion
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestId = test.Id,
                TestQuestion = "question??"
            };
            var questionAnswer = new MongoQuestionAnswer
            {
                Id = 1,
                Answer = "it is true",
                IsCorrect = true,
                QuestionId = question.Id
            };

            var insertTasks = new List<Task>
            {
                context.Tests.InsertOneAsync(test),
                context.Questions.InsertOneAsync(question),
                context.Answers.InsertOneAsync(questionAnswer),
            };

            var query = new GetTestToPassQuery
            {
                Id = 1
            };

            var expectedResult = new TestToPassResponse
            {
                Id = 1,
                Description = "description",
                Name = "test name",
                TestType = TestType.Timed.ToString(),
                Questions = new List<QuestionToPassTest>
                {
                    new QuestionToPassTest
                    {
                        Id = 1,
                        QuestionType = QuestionType.OneAnswer.ToString(),
                        TestQuestion = "question??",
                        Answers = new List<QuestionAnswerToPassTest>
                        {
                            new QuestionAnswerToPassTest
                            {
                                Id = 1,
                                Answer = "it is true"
                            }
                        }
                    }
                }
            };

            // Act
            var testToPass = await sender.Send(query, CancellationToken.None);

            // Assert
            testToPass.Should().BeEquivalentTo(expectedResult);
        }
    }
}
