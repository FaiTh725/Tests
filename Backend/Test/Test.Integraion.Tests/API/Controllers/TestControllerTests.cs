using Docker.DotNet.Models;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using MongoDB.Driver;
using System.Net;
using Test.API.Contracts.Test;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Integration.Tests.Extensions;
using Test.Integration.Tests.JwtToken;

namespace Test.Integration.Tests.API.Controllers
{
    public class TestControllerTests : BaseIntegrationTest
    {
        public TestControllerTests(CustomWebFactory factory) 
            : base(factory)
        {}

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
        [InlineData("name", "description", TestType.Timed, null)]
        public async Task CreateTest_WhenIncorrectRequest_ShouldReturns400Status(
            string name, 
            string description, 
            TestType type, 
            double? duration)
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
    }
}
