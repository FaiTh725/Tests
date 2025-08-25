using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using Test.API.Contracts.Test;
using Test.Application.Commands.Test.SendTestAnswer;
using Test.Application.Commands.Test.StopTest;
using Test.Application.Contracts.ProfileAnswerEntity;
using Test.Application.Contracts.TestSession;
using Test.Dal.Persistences;
using Test.Domain.Enums;
using Test.Integration.Tests.Extensions;
using Test.Integration.Tests.JwtToken;

namespace Test.Integration.Tests.API.Controllers
{
    [Collection("Integration Tests")]
    public class TestSessionControllerTests : 
        BaseIntegrationTest
    {
        public TestSessionControllerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task StartTest_WhenTestDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new StartTestRequest
            {
                TestId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/StartTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task StartTest_WhenTestExists_ShouldReturns200Status()
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
                CreatedTime = DateTime.Now,
                Description = "description",
                Name = "name",
                Id = 1,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                IsPublic = true,
                ProfileId = profile.Id
            };

            var insertTasks = new List<Task>()
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(test)
            };

            await Task.WhenAll(insertTasks);

            var request = new StartTestRequest
            {
                TestId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/StartTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var testSessions = (await tempDbService.GetAllEntities()).ToList();

            testSessions.Should().NotBeEmpty();
            testSessions[0].ProfileId.Should().Be(profile.Id);
            testSessions[0].TestId.Should().Be(test.Id);
            testSessions[0].TestDuration.Should().Be(test.DurationInMinutes);
        }

        [Fact]
        public async Task StartTest_WhenTestIsPrivate_ShouldReturns403Status()
        {
            // Arrange
            var createrTest = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            var profile1 = new MongoProfile
             {
                 Email = "test123@mail.com",
                 Name = "test",
                 Id = 2
             };
            var test = new MongoTest
            {
                CreatedTime = DateTime.Now,
                Description = "description",
                Name = "name",
                Id = 1,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                IsPublic = false,
                ProfileId = createrTest.Id
            };

            var insertTasks = new List<Task>()
            {
                context.Profiles.InsertOneAsync(createrTest),
                context.Profiles.InsertOneAsync(profile1),
                context.Tests.InsertOneAsync(test)
            };

            await Task.WhenAll(insertTasks);

            var request = new StartTestRequest
            {
                TestId = 1
            };

            var user = new JwtUserData
            {
                Email = "test123@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/StartTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task SendTestAnswer_WhenSessionDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var createrTest = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            var test = new MongoTest
            {
                CreatedTime = DateTime.Now,
                Description = "description",
                Name = "name",
                Id = 1,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                IsPublic = false,
                ProfileId = createrTest.Id
            };

            var insertTasks = new List<Task>()
            {
                context.Profiles.InsertOneAsync(createrTest),
                context.Tests.InsertOneAsync(test)
            };

            await Task.WhenAll(insertTasks);

            var request = new SendTestAnswerCommand
            {
                QuestionAnswersId = [],
                QuestionId = 1,
                SessionId = Guid.NewGuid()
            };

            var user = new JwtUserData
            {
                Email = "test123@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/SendTestAnswer", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendTestAnswer_WhenAnswerContainsInvalidAnswersId_ShouldReturns400Status()
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
                CreatedTime = DateTime.Now,
                Description = "description",
                Name = "name",
                Id = 1,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                IsPublic = false,
                ProfileId = profile.Id
            };

            var insertTasks = new List<Task>()
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(test)
            };

            await Task.WhenAll(insertTasks);

            var testSessionId = Guid.NewGuid();
            var testSession = new TempTestSession
            {
                Answers = [],
                Id = testSessionId,
                JobId = string.Empty,
                ProfileId = profile.Id,
                StartTime = DateTime.Now,
                TestDuration = 15,
                TestId = test.Id
            };

            await tempDbService.AddEntity(testSession);

            var request = new SendTestAnswerCommand
            {
                QuestionAnswersId = [1, 2],
                QuestionId = 1,
                SessionId = testSessionId
            };

            var user = new JwtUserData
            {
                Email = "test123@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/SendTestAnswer", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendTestAnswer_WhenRequestIsCorrect_ShouldReturns204Status()
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
                CreatedTime = DateTime.Now,
                Description = "description",
                Name = "name",
                Id = 1,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                IsPublic = false,
                ProfileId = profile.Id
            };

            var testQuestion = new MongoQuestion
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestId = test.Id,
                TestQuestion = "test question text"
            };
            var questionAnswer = new MongoQuestionAnswer
            {
                Answer = "is correct",
                Id = 1,
                IsCorrect = true,
                QuestionId = testQuestion.Id,
            };

            var insertTasks = new List<Task>()
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(test),
                context.Questions.InsertOneAsync(testQuestion),
                context.Answers.InsertOneAsync(questionAnswer)
            };

            await Task.WhenAll(insertTasks);

            var testSessionId = Guid.NewGuid();
            var testSession = new TempTestSession
            {
                Answers = [],
                Id = testSessionId,
                JobId = string.Empty,
                ProfileId = profile.Id,
                StartTime = DateTime.Now,
                TestDuration = 15,
                TestId = test.Id
            };

            await tempDbService.AddEntity(testSession);

            var request = new SendTestAnswerCommand
            {
                QuestionAnswersId = [1],
                QuestionId = 1,
                SessionId = testSessionId
            };

            var user = new JwtUserData
            {
                Email = "test123@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/SendTestAnswer", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var existedTestSession = await tempDbService.GetEntity(testSessionId);

            existedTestSession.Should().NotBeNull();
            existedTestSession.Answers.Should().HaveCount(1);
            existedTestSession.Answers[0].QuestionId.Should().Be(testQuestion.Id);
            existedTestSession.Answers[0].QuestionAnswersId.Should()
                .BeEquivalentTo(request.QuestionAnswersId);
        }

        [Fact]
        public async Task StopTest_WhenSessionDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new StopTestCommand
            {
                SessionId = Guid.NewGuid()
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/StopTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task StopTest_WhenUserAnswerAnyQuestions_ShouldReturns200Status()
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
                CreatedTime = DateTime.Now,
                Description = "description",
                Name = "name",
                Id = 1,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                IsPublic = false,
                ProfileId = profile.Id
            };

            var testQuestion = new MongoQuestion
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestId = test.Id,
                TestQuestion = "test question text"
            };
            var questionAnswer = new MongoQuestionAnswer
            {
                Answer = "is correct",
                Id = 1,
                IsCorrect = true,
                QuestionId = testQuestion.Id,
            };

            var insertTasks = new List<Task>()
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(test),
                context.Questions.InsertOneAsync(testQuestion),
                context.Answers.InsertOneAsync(questionAnswer)
            };

            await Task.WhenAll(insertTasks);

            var testSessionId = Guid.NewGuid();
            var testSession = new TempTestSession
            {
                Answers = new List<SessionProfileAnswer>
                {
                    new SessionProfileAnswer
                    {
                        QuestionAnswersId = [questionAnswer.Id],
                        QuestionId = testQuestion.Id,
                        SendTime = DateTime.Now,
                    }
                },
                Id = testSessionId,
                JobId = string.Empty,
                ProfileId = profile.Id,
                StartTime = DateTime.Now,
                TestDuration = 15,
                TestId = test.Id
            };

            await tempDbService.AddEntity(testSession);

            var request = new StopTestCommand
            {
                SessionId = testSessionId
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/StopTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var closedSession = await tempDbService
                .GetEntity(testSessionId);
        
            closedSession.Should().BeNull();

            var savedTestSessin = await context.Sessions
                .Find(_ => true)
                .SingleOrDefaultAsync();

            savedTestSessin.Should().NotBeNull();
            savedTestSessin.IsEnded.Should().BeTrue();
            savedTestSessin.TestId.Should().Be(test.Id);
            savedTestSessin.Percent.Should().Be(100);
            savedTestSessin.ProfileId.Should().Be(profile.Id);
        }

        [Fact]
        public async Task StopTest_WhenUserDoesntAnswerAnyQuestions_ShouldReturns200Status()
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
                CreatedTime = DateTime.Now,
                Description = "description",
                Name = "name",
                Id = 1,
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                IsPublic = false,
                ProfileId = profile.Id
            };

            var testQuestion = new MongoQuestion
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestId = test.Id,
                TestQuestion = "test question text"
            };
            var questionAnswer = new MongoQuestionAnswer
            {
                Answer = "is correct",
                Id = 1,
                IsCorrect = true,
                QuestionId = testQuestion.Id,
            };

            var insertTasks = new List<Task>()
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(test),
                context.Questions.InsertOneAsync(testQuestion),
                context.Answers.InsertOneAsync(questionAnswer)
            };

            await Task.WhenAll(insertTasks);

            var testSessionId = Guid.NewGuid();
            var testSession = new TempTestSession
            {
                Answers = new List<SessionProfileAnswer>
                {
                    new SessionProfileAnswer
                    {
                        QuestionAnswersId = [],
                        QuestionId = testQuestion.Id,
                        SendTime = DateTime.Now,
                    }
                },
                Id = testSessionId,
                JobId = string.Empty,
                ProfileId = profile.Id,
                StartTime = DateTime.Now,
                TestDuration = 15,
                TestId = test.Id
            };

            await tempDbService.AddEntity(testSession);

            var request = new StopTestCommand
            {
                SessionId = testSessionId
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/TestSession/StopTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var closedSession = await tempDbService
                .GetEntity(testSessionId);

            closedSession.Should().BeNull();

            var savedTestSessin = await context.Sessions
                .Find(_ => true)
                .SingleOrDefaultAsync();

            savedTestSessin.Should().NotBeNull();
            savedTestSessin.IsEnded.Should().BeTrue();
            savedTestSessin.TestId.Should().Be(test.Id);
            savedTestSessin.Percent.Should().Be(0);
            savedTestSessin.ProfileId.Should().Be(profile.Id);
        }
    }
}
