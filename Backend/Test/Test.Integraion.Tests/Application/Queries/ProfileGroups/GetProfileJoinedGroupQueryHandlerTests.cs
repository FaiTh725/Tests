using FluentAssertions;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup;
using Test.Application.Queries.ProfileGroupEntity.GetProfileJoinedGroup;
using Test.Dal.Persistences;

namespace Test.Integration.Tests.Application.Queries.ProfileGroups
{
    [Collection("Integration Tests")]
    public class GetProfileJoinedGroupQueryHandlerTests : 
        BaseIntegrationTest
    {
        public GetProfileJoinedGroupQueryHandlerTests(CustomWebFactory factory): 
            base(factory)
        {}

        [Fact]
        public async Task Handle_ReturnProfileJoinedGroups()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };
            var member = new MongoProfile
            {
                Email = "test123@mail.com",
                Id = 2,
                Name = "test"
            };

            var group = new MongoProfileGroup
            {
                GroupName = "group name",
                Id = 1,
                MembersId = [2],
                OwnerId = profile.Id
            };

            var insertTask = new List<Task>
            {
                context.Profiles.InsertManyAsync([profile, member]),
                context.Groups.InsertOneAsync(group),
            };

            var query = new GetProfileJoinedGroupQuery
            {
                ProfileId = member.Id
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
