using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using Test.API.Contracts.Test;
using Test.API.Contracts.TestAccess;
using Test.Dal.Persistences;
using Test.Domain.Enums;
using Test.Integration.Tests.Extensions;
using Test.Integration.Tests.JwtToken;
using Xunit.Abstractions;

namespace Test.Integration.Tests.API.Controllers
{
    [Collection("Integration Tests")]
    public class TestControllerTests : BaseIntegrationTest
    {
        private readonly ITestOutputHelper testLogger;

        public TestControllerTests(
            CustomWebFactory factory,
            ITestOutputHelper testLogger) 
            : base(factory)
        {
            this.testLogger = testLogger;
        }

        [Fact]
        public async Task CreateTest_WhenProfileDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var request = new CreateTestRequest()
            {
                Description = "description",
                Name = "name",
                DurationInMinutes = 15,
                IsPublic = true,
                TestType = TestType.Timed
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/CreateTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("", "description", TestType.Timed, 15)]
        [InlineData("name", "", TestType.Timed, 15)]
        [InlineData("name", "description", TestType.Progressive, 15)]
        public async Task CreateTest_WhenIncorrectRequest_ShouldReturns400Status(
            string name, 
            string description, 
            TestType type, 
            double duration)
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new CreateTestRequest
            {
                Name = name,
                Description = description,
                TestType = type,
                DurationInMinutes = duration,
                IsPublic = true
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/CreateTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateTest_WhenRequestIsCorrect_ShouldReturns200StatusAndCreateTest()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new CreateTestRequest
            {
                Name = "test",
                Description = "description",
                TestType = TestType.Timed,
                DurationInMinutes = 15,
                IsPublic = true
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/CreateTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var addedtest = await context.Tests
                .Find(_ => true)
                .SingleOrDefaultAsync();

            addedtest.Should().NotBeNull();
            addedtest.Name.Should().Be(request.Name);
            addedtest.Description.Should().Be(request.Description);
            addedtest.IsPublic.Should().Be(request.IsPublic);
            addedtest.TestType.Should().Be(request.TestType);
            addedtest.DurationInMinutes.Should().Be(request.DurationInMinutes);
            addedtest.ProfileId.Should().Be(profile.Id);
        }

        [Fact]
        public async Task DeleteTest_WhenTestDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var testIdToDelete = 1;

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Test/DeleteTest", testIdToDelete, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteTest_WhenTestExists_ShouldReturns400Status()
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
            var testQuestion = new MongoQuestion
            {
                Id = 1,
                QuestionType = QuestionType.OneAnswer,
                QuestionWeight = 12,
                TestId = existedTest.Id,
                TestQuestion = "test question text"
            };
            var questionAnswer = new MongoQuestionAnswer
            {
                Answer = "is correct",
                Id = 1,
                IsCorrect = true,
                QuestionId = testQuestion.Id,
            };

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest),
                context.Questions.InsertOneAsync(testQuestion),
                context.Answers.InsertOneAsync(questionAnswer),
            };

            await Task.WhenAll(insertDataTasks);

            var testIdToDelete = 1;

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync($"/api/Test/DeleteTest?testId={testIdToDelete}", testIdToDelete, user);

            // Assert
            testLogger.WriteLine(await httpResponse.Content.ReadAsStringAsync());
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await WaitOutboxMessages();
            var deletedTest = await context.Tests.Find(_ => true).SingleOrDefaultAsync();
            deletedTest.Should().BeNull();
            var deletedQuestion = await context.Questions.Find(_ => true).SingleOrDefaultAsync();
            deletedQuestion.Should().BeNull();
            var deletexQuestionAnswer = await context.Answers.Find(_ => true).SingleOrDefaultAsync();
            deletexQuestionAnswer.Should().BeNull();
        }

        [Theory]
        [InlineData("", "description", TestType.Timed, 15)]
        [InlineData("Name", "", TestType.Timed, 15)]
        [InlineData("Name", "", TestType.Progressive, 15)]
        public async Task UpdateTest_WhenRequestContainsInvalidData_ShouldReturns400Status(
            string name, string description, TestType testType, double duration)
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
                TestType = testType,
                DurationInMinutes = duration,
                CreatedTime = DateTime.UtcNow
            };

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new UpdateTestRequest()
            { 
                Name = name,
                Description = description,
                IsPublic = true,
                TestId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Test/UpdateTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateTest_WhenTestDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new UpdateTestRequest()
            {
                Name = "name",
                Description = "description",
                IsPublic = true,
                TestId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Test/UpdateTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateTest_WhenTestExistsAndRequestValid_ShouldReturns200Status()
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

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new UpdateTestRequest()
            {
                Name = "new name",
                Description = "new description",
                IsPublic = true,
                TestId = 1,
                TestType = TestType.Timed,
                TestDuration = 15
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Test/UpdateTest", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedTest = await context.Tests
                .Find(_ => true)
                .SingleOrDefaultAsync();

            updatedTest.Should().NotBeNull();
            updatedTest.Name.Should().Be(request.Name);
            updatedTest.Description.Should().Be(request.Description);
        }

        [Fact]
        public async Task ProvideTestAccess_WhenTestDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new ProvideTestAccessRequest()
            {
                TestId = 1,
                TargetAccessEntityType = TargetAccessEntityType.Profile,
                TargetEntityId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/ProviderTestAccess", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ProvideTestAccess_WhenEntityAlreadyHasAccess_ShouldReturns409Status()
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
            var testAccess = new MongoTestAccess
            {
                TargetEntityId = 1,
                Id = 1,
                TestId = 1,
                TargetAccessEntityType = TargetAccessEntityType.Profile
            };

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest),
                context.Accesses.InsertOneAsync(testAccess)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new ProvideTestAccessRequest()
            {
                TestId = 1,
                TargetAccessEntityType = TargetAccessEntityType.Profile,
                TargetEntityId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/ProviderTestAccess", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task ProvideTestAccess_WhenTargetEntityDoesntExist_ShouldReturns400Status()
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

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new ProvideTestAccessRequest()
            {
                TestId = 1,
                TargetAccessEntityType = TargetAccessEntityType.Group,
                TargetEntityId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/ProviderTestAccess", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ProvideTestAccess_WhenRequestIsCorrect_ShouldReturns200Status()
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

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new ProvideTestAccessRequest()
            {
                TestId = 1,
                TargetAccessEntityType = TargetAccessEntityType.Profile,
                TargetEntityId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/ProviderTestAccess", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var testAccess = await context.Accesses
                .Find(_ => true)
                .SingleOrDefaultAsync();

            testAccess.Should().NotBeNull();
            testAccess.TestId.Should().Be(request.TestId);
            testAccess.TargetEntityId.Should().Be(request.TargetEntityId);
            testAccess.TargetAccessEntityType.Should().Be(request.TargetAccessEntityType);
        }

        [Fact]
        public async Task LimitTestAccess_WhenAccessDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Name = "test",
                Id = 1
            };
            
            await context.Profiles.InsertOneAsync(profile);

            var request = new LimitTestAccessRequest()
            {
                TestId = 1,
                TargetAccessEntityType = TargetAccessEntityType.Profile,
                TargetEntityId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/LimitTestAccess", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task LimitTestAccess_WhenAccessExist_ShouldReturns204Status()
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
            var testAccess = new MongoTestAccess
            {
                TargetEntityId = 1,
                Id = 1,
                TestId = 1,
                TargetAccessEntityType = TargetAccessEntityType.Profile
            };

            var insertDataTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(existedTest),
                context.Accesses.InsertOneAsync(testAccess)
            };

            await Task.WhenAll(insertDataTasks);

            var request = new LimitTestAccessRequest()
            {
                TestId = 1,
                TargetAccessEntityType = TargetAccessEntityType.Profile,
                TargetEntityId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Test/LimitTestAccess", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedTestAccess = await context.Accesses
                .Find(_ => true)
                .SingleOrDefaultAsync();

            deletedTestAccess.Should().BeNull();
        }
    }
}
