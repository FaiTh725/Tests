using FluentAssertions;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup;
using Test.Dal.Persistences;

namespace Test.Integration.Tests.Application.Queries.ProfileGroups
{
    [Collection("Integration Tests")]
    public class GetProfileCreatedGroupQueryHandlerTests : 
        BaseIntegrationTest
    {
        public GetProfileCreatedGroupQueryHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenProfileDoesntCreateGroup_ShouldReturnEmptyList()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };

            await context.Profiles.InsertOneAsync(profile);

            var query = new GetProfileCreatedGroupQuery
            {
                ProfileEmail = profile.Email
            };

            // Act
            var groups = await sender.Send(query, CancellationToken.None);

            // Assert
            groups.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenProfileCreatedGroup_ShouldReturnListGroups()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };

            var group = new MongoProfileGroup
            {
                GroupName = "group name",
                Id = 1,
                MembersId = [],
                OwnerId = profile.Id
            };

            var insertTask = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Groups.InsertOneAsync(group),
            };

            var query = new GetProfileCreatedGroupQuery
            {
                ProfileEmail = profile.Email
            };

            var expectedGroups = new List<GroupInfo>
            { 
                new GroupInfo
                {
                    Id = 1,   
                    Name = "group name"
                }
            };


            // Act
            var groups = await sender.Send(query, CancellationToken.None);

            // Assert
            groups.Should().BeEquivalentTo(expectedGroups);
        }
    }
}
