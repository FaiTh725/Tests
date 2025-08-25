using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using Test.API.Contracts.ProfileGroupEntity;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Integration.Tests.Extensions;
using Test.Integration.Tests.JwtToken;
using Xunit.Abstractions;

namespace Test.Integration.Tests.API.Controllers
{
    [Collection("Integration Tests")]
    public class GroupControllerTests : 
        BaseIntegrationTest
    {
        private readonly ITestOutputHelper testLogger;

        public GroupControllerTests(
            CustomWebFactory factory,
            ITestOutputHelper testLogger) : 
            base(factory)
        {
            this.testLogger = testLogger;
        }

        [Fact]
        public async Task CreateGroup_WhenInvalidRequest_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new CreateGroupRequest
            {
                Name = ""
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Group/CreateGroup", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateGroup_WhenRequestCorrect_ShouldReturns200Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new CreateGroupRequest
            {
                Name = "test group"
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Group/CreateGroup", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var group = await context.Groups
                .Find(_ => true)
                .SingleOrDefaultAsync();

            group.Should().NotBeNull();
            group.GroupName.Should().Be(request.Name);
            group.OwnerId.Should().Be(profile.Id);
        }

        [Fact]
        public async Task DeleteGroup_WhenGroupDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new DeleteGroupRequest
            {
                GroupId = 1
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Group/DeleteGroup", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteGroup_WhenUserDoesntHavePermissions_ShouldReturns403Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };
            var notOwnerProfile = new MongoProfile
            {
                Email = "test123@mail.com",
                Id = 2,
                Name = "test"
            };
            var group = new MongoProfileGroup
            {
                GroupName = "test",
                Id = 1,
                MembersId = [],
                OwnerId = profile.Id
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertManyAsync([profile, notOwnerProfile]),
                context.Groups.InsertOneAsync(group)
            };

            await Task.WhenAll(insertTasks);

            var request = new DeleteGroupRequest
            {
                GroupId = 1
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test123@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Group/DeleteGroup", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteGroup_WhenRequestCorrect_ShouldReturns204Status()
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
                GroupName = "test",
                Id = 1,
                MembersId = [],
                OwnerId = profile.Id
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Groups.InsertOneAsync(group)
            };

            await Task.WhenAll(insertTasks);

            var request = new DeleteGroupRequest
            {
                GroupId = group.Id
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Group/DeleteGroup", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedGroup = await context.Groups
                .Find(x => x.Id == group.Id)
                .FirstOrDefaultAsync();

            deletedGroup.Should().BeNull();
        }

        [Fact]
        public async Task AddGroupMember_WhenGroupDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new AddGroupMemberRequest
            {
                GroupId = 1,
                MemberId = 1
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Group/AddGroupMember", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddGroupMember_WhenProfileDoesntExist_ShouldReturns400Status()
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
                GroupName = "test",
                Id = 1,
                MembersId = [],
                OwnerId = profile.Id
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Groups.InsertOneAsync(group)
            };

            await Task.WhenAll(insertTasks);

            var request = new AddGroupMemberRequest
            {
                MemberId = 2,
                GroupId = group.Id
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Group/AddGroupMember", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddGroupMember_WhenProfileInGroup_ShouldReturns409Status()
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
                GroupName = "test",
                Id = 1,
                MembersId = [1],
                OwnerId = profile.Id
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Groups.InsertOneAsync(group)
            };

            await Task.WhenAll(insertTasks);

            var request = new AddGroupMemberRequest
            {
                MemberId = 1,
                GroupId = group.Id
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Group/AddGroupMember", request, user);

            // Assert
            
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task AddGroupMember_WhenProfileIsntInGroup_ShouldReturns204Status()
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
                GroupName = "test",
                Id = 1,
                MembersId = [],
                OwnerId = profile.Id
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertManyAsync([profile, member]),
                context.Groups.InsertOneAsync(group)
            };

            await Task.WhenAll(insertTasks);

            var request = new AddGroupMemberRequest
            {
                MemberId = member.Id,
                GroupId = group.Id
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Group/AddGroupMember", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await WaitOutboxMessages();

            var updatedGroup = await context.Groups
                .Find(x => x.Id == group.Id)
                .FirstOrDefaultAsync();

            updatedGroup.Should().NotBeNull();
            updatedGroup.MembersId.Should().BeEquivalentTo(new List<long>() { member.Id });
        }

        [Fact]
        public async Task DeleteMembersGroup_WhenGroupDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = 1,
                Name = "test"
            };

            await context.Profiles.InsertOneAsync(profile);

            var request = new DeleteMembersGroupRequest 
            { 
                GroupId = 1,
                MembersId = [1]
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Group/DeleteMembersGroup", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteMembersGroup_WhenMemberDoesntInGroup_ShouldReturns400Status()
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
                GroupName = "test",
                Id = 1,
                MembersId = [],
                OwnerId = profile.Id
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Groups.InsertOneAsync(group)
            };

            await Task.WhenAll(insertTasks);

            var request = new DeleteMembersGroupRequest
            {
                GroupId = group.Id,
                MembersId = [4]
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Group/DeleteMembersGroup", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteMembersGroup_WhenMembersInGroup_ShouldReturns204Status()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Email = "test@mail.com",
                Id = context.GetNextId("profiles"),
                Name = "test"
            };
            var member = new MongoProfile
            {
                Id = context.GetNextId("profiles"),
                Email = "test123@mail.com",
                Name = "test"
            };
            var group = new MongoProfileGroup
            {
                GroupName = "test",
                Id = 1,
                MembersId = [member.Id],
                OwnerId = profile.Id
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertManyAsync([profile, member]),
                context.Groups.InsertOneAsync(group)
            };

            await Task.WhenAll(insertTasks);

            var request = new DeleteMembersGroupRequest
            {
                GroupId = group.Id,
                MembersId = [member.Id]
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Group/DeleteMembersGroup", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updatedGroup = await context.Groups
                .Find(x => x.Id == group.Id)
                .FirstOrDefaultAsync();

            updatedGroup.Should().NotBeNull();
            updatedGroup.MembersId.Should().BeEquivalentTo(new List<long>());
        }
    }
}
