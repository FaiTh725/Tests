using FluentAssertions;
using System;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.Test.GetProfileTests;
using Test.Dal.Persistences;
using Test.Domain.Enums;

namespace Test.Integration.Tests.Application.Queries.Tests
{
    [Collection("Integration Tests")]
    public class GetProfileTestsQueryHandlerTests : 
        BaseIntegrationTest
    {
        public GetProfileTestsQueryHandlerTests(CustomWebFactory factory) :
            base(factory)
        {}

        [Fact]
        public async Task Handle_ReturnsProfileCreatedTests()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };

            var test = new MongoTest
            {
                Name = "test",
                Id = 1,
                CreatedTime = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                ProfileId = profile.Id,
                Description = "description",
                DurationInMinutes = 15,
                IsPublic = true,
                TestType = TestType.Timed
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Tests.InsertOneAsync(test)
            };

            await Task.WhenAll(insertTasks);

            var query = new GetProfileTestsQuery
            {
                ProfileEmail = "test@mail.com"
            };

            var expectedResult = new List<TestInfo>
            {
                new TestInfo
                {
                    Id = 1,
                    Name = "test",
                    CreatedTime = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                    Description = "description",
                    DurationInMinutes = 15,
                    IsPublic = true,
                    TestType = TestType.Timed.ToString(),
                    Owner = new ProfileResponse
                    {
                        Id = 1,
                        Email = "test@mail.com",
                        Name = "test"
                    }
                }
            };

            // Act
            var tests = await sender.Send(query, CancellationToken.None);

            // Assert
            tests.Should().BeEquivalentTo(expectedResult);
        }
    }
}
