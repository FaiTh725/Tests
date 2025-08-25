using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Text.Json;
using Test.API.Contracts.Question;
using Test.API.Contracts.QuestionAnswer;
using Test.Dal.Persistences;
using Test.Domain.Enums;
using Test.Integration.Tests.Extensions;
using Test.Integration.Tests.JwtToken;
using Xunit.Abstractions;

namespace Test.Integration.Tests.API.Controllers
{
    [Collection("Integration Tests")]
    public class QuestionControllerTests : 
        BaseIntegrationTest
    {
        private readonly ITestOutputHelper testLogger;
        public QuestionControllerTests(
            CustomWebFactory factory, 
            ITestOutputHelper testLogger) : 
            base(factory)
        {
            this.testLogger = testLogger;
        }

        [Fact]
        public async Task AddQuestion_WhenTestDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new CreateQuestionRequest
            { 
                Answers = new List<CreateQuestionAnswerRequest>(),
                QuestionType = QuestionType.ManyAnswers,
                TestId = 1,
                TestQuestion = "text",
                QuestionWeight = 12
            };
            var formData = new MultipartFormDataContent
            {
                { new StringContent(request.QuestionType.ToString()), "QuestionType" },
                { new StringContent(request.TestId.ToString()), "TestId" },
                { new StringContent(request.TestQuestion.ToString()), "TestQuestion" },
                { new StringContent(request.QuestionWeight.ToString()), "QuestionWeight" },
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostFormData("/api/Question/AddQuestion", formData, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddQuestion_WhenManyAnswerQuestionDoesntContainsRightAnswers_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            var existedTest = new MongoTest
            {
                Id = 2,
                Name = "test",
                Description = "test",
                ProfileId = profile.Id,
                IsPublic = true,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                CreatedTime = DateTime.UtcNow
            };

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new CreateQuestionRequest
            {
                Answers = new List<CreateQuestionAnswerRequest>(),
                QuestionType = QuestionType.ManyAnswers,
                TestId = existedTest.Id,
                TestQuestion = "text",
                QuestionWeight = 12
            };
            var formData = new MultipartFormDataContent
            {
                { new StringContent(request.QuestionType.ToString()), "QuestionType" },
                { new StringContent(request.TestId.ToString()), "TestId" },
                { new StringContent(request.TestQuestion.ToString()), "TestQuestion" },
                { new StringContent(request.QuestionWeight.ToString()), "QuestionWeight" },
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostFormData("/api/Question/AddQuestion", formData, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddQuestion_WhenOneAnswerQuestiontContainsManyRightAnswers_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            var existedTest = new MongoTest
            {
                Id = 3,
                Name = "test",
                Description = "test",
                ProfileId = profile.Id,
                IsPublic = true,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                CreatedTime = DateTime.UtcNow
            };

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new CreateQuestionRequest
            {
                Answers = new List<CreateQuestionAnswerRequest>()
                {
                    new CreateQuestionAnswerRequest
                    {
                        Answer = "is correct",
                        IsCorrect = true,
                    },
                    new CreateQuestionAnswerRequest
                    {
                        Answer = "is correct too",
                        IsCorrect = true,
                    },
                },
                QuestionType = QuestionType.OneAnswer,
                TestId = existedTest.Id,
                TestQuestion = "text",
                QuestionWeight = 12
            };
            var formData = new MultipartFormDataContent
            {
                { new StringContent(request.QuestionType.ToString()), "QuestionType" },
                { new StringContent(request.TestId.ToString()), "TestId" },
                { new StringContent(request.TestQuestion.ToString()), "TestQuestion" },
                { new StringContent(request.QuestionWeight.ToString()), "QuestionWeight" },
                { new StringContent(request.Answers[0].IsCorrect.ToString()), "Answers[0].IsCorrect" },
                { new StringContent(request.Answers[0].Answer.ToString()), "Answers[0].Answer" },
                { new StringContent(request.Answers[1].Answer.ToString()), "Answers[1].Answer" },
                { new StringContent(request.Answers[1].IsCorrect.ToString()), "Answers[1].IsCorrect" },
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostFormData("/api/Question/AddQuestion", formData, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddQuestion_WhenRequestIsCorrect_ShouldReturns200Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            var existedTest = new MongoTest
            {
                Id = 5,
                Name = "test",
                Description = "test",
                ProfileId = profile.Id,
                IsPublic = true,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                CreatedTime = DateTime.UtcNow
            };

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new CreateQuestionRequest
            {
                Answers = new List<CreateQuestionAnswerRequest>()
                {
                    new CreateQuestionAnswerRequest
                    {
                        Answer = "is correct",
                        IsCorrect = true,
                    }
                },
                QuestionType = QuestionType.OneAnswer,
                TestId = existedTest.Id,
                TestQuestion = "text",
                QuestionWeight = 12
            };
            var formData = new MultipartFormDataContent
            {
                { new StringContent(request.QuestionType.ToString()), "QuestionType" },
                { new StringContent(request.TestId.ToString()), "TestId" },
                { new StringContent(request.TestQuestion.ToString()), "TestQuestion" },
                { new StringContent(request.QuestionWeight.ToString()), "QuestionWeight" },
                { new StringContent(request.Answers[0].IsCorrect.ToString()), "Answers[0].IsCorrect" },
                { new StringContent(request.Answers[0].Answer.ToString()), "Answers[0].Answer" },
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostFormData("/api/Question/AddQuestion", formData, user);

            // Assert
            testLogger.WriteLine(await httpResponse.Content.ReadAsStringAsync());
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            await WaitOutboxMessages();

            var question = await context.Questions
                .Find(_ => true)
                .SingleOrDefaultAsync();
        
            question.Should().NotBeNull();
            question.QuestionWeight.Should().Be(request.QuestionWeight);
            question.TestId.Should().Be(request.TestId);
            question.QuestionType.Should().Be(request.QuestionType);
            question.TestQuestion.Should().Be(request.TestQuestion);

            var answer = await context.Answers
                .Find(_ => true)
                .SingleOrDefaultAsync();

            answer.Should().NotBeNull();
            answer.IsCorrect.Should().BeTrue();
            answer.Answer.Should().Be(request.Answers[0].Answer);
            answer.QuestionId.Should().Be(question.Id);
        }

        [Fact]
        public async Task DeleteQuestion_WhenQuestionDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var questionToDelete = 1; 

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync($"/api/Question/DeleteQuestion?questionId={questionToDelete}", user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteQuestion_WhenQuestionExists_ShouldReturns204Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            var existedTest = new MongoTest
            {
                Id = 1,
                Name = "test",
                Description = "test",
                ProfileId = profile.Id,
                IsPublic = true,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                CreatedTime = DateTime.UtcNow
            };
            var existedQuestion = new MongoQuestion
            {
                Id= 1,
                QuestionType = QuestionType.OneAnswer,
                TestId = 1,
                QuestionWeight = 12,
                TestQuestion = "question?"
            };
            var existedQuestionAnswer = new MongoQuestionAnswer
            {
                IsCorrect = true,
                Id = 1,
                Answer = "is correct answer",
                QuestionId = 1
            };

            var insertDataTask = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest),
                context.Questions.InsertOneAsync(existedQuestion),
                context.Answers.InsertOneAsync(existedQuestionAnswer),
            };

            await Task.WhenAll(insertDataTask);

            var questionToDelete = 1;

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync($"/api/Question/DeleteQuestion?questionId={questionToDelete}", user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await WaitOutboxMessages();

            var question = await context.Questions
                .Find(_ => true)
                .SingleOrDefaultAsync();

            question.Should().BeNull();

            var answer = await context.Answers
                .Find(_ => true)
                .SingleOrDefaultAsync();

            answer.Should().BeNull();
        }

        [Fact]
        public async Task UpdateQuestion_WhenQuestionDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new UpdateQuestionRequest
            {
                Id = 1,
                QuestionWeight = 12,
                TestQuestion = "test question"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Question/UpdateQuestion", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("", 12)]
        [InlineData("question", -2)]
        public async Task UpdateQuestion_WhenIncorrectRequest_ShouldReturns400Status(
            string question, int weight)
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            var existedQuestion = new MongoQuestion
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestId = 1,
                TestQuestion = "old question"
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Questions.InsertOneAsync(existedQuestion),
            };

            await Task.WhenAll(insertTasks);

            var request = new UpdateQuestionRequest
            {
                Id = existedQuestion.Id,
                QuestionWeight = weight,
                TestQuestion = question
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Question/UpdateQuestion", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateQuestion_WhenRequestCorrect_ShouldReturns200Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            var test = new MongoTest
            {
                CreatedTime = DateTime.UtcNow,
                Description = "description",
                Id = 1,
                IsPublic = true,
                DurationInMinutes = 15,
                Name = "name",
                ProfileId = profile.Id,
                TestType = TestType.Timed
            };
            var existedQuestion = new MongoQuestion
            {
                Id = 10,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestId = test.Id,
                TestQuestion = "old question"
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(test),
                context.Questions.InsertOneAsync(existedQuestion),
            };

            await Task.WhenAll(insertTasks);

            var request = new UpdateQuestionRequest
            {
                Id = existedQuestion.Id,
                QuestionWeight = 3,
                TestQuestion = "new question"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Question/UpdateQuestion", request, user);

            // Assert
            testLogger.WriteLine(await httpResponse.Content.ReadAsStringAsync());
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedQuestion = await context.Questions
                .Find(_ => true)
                .FirstOrDefaultAsync();

            updatedQuestion.Should().NotBeNull();
            updatedQuestion.TestQuestion.Should().Be(request.TestQuestion);
            updatedQuestion.QuestionWeight.Should().Be(request.QuestionWeight);
        }
    }
}
