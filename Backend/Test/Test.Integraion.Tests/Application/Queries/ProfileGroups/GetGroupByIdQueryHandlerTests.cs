using Application.Shared.Exceptions;
using FluentAssertions;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.GetGroupById;
using Test.Dal.Persistences;
using Test.Domain.Entities;

namespace Test.Integration.Tests.Application.Queries.ProfileGroups
{
    [Collection("Integration Tests")]
    public class GetGroupByIdQueryHandlerTests : 
        BaseIntegrationTest
    {
        public GetGroupByIdQueryHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenGroupDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetGroupByIdQuery
            {
                Id = 1
            };

            // Act
            var act = async () => await sender.Send(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Profile group doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenGroupExist_ShouldReturnProfileGroupWithOwner()
        {
            // Arrange
            var profile = new MongoProfile
            {
                Id = 1,
                Email = "test@mail.com",
                Name = "test"
            };
            
            var group = new MongoProfileGroup
            {
                Id = 1,
                GroupName = "test",
                MembersId = [],
                OwnerId = 1
            };

            var insertTasks = new List<Task>
            {
                context.Profiles.InsertOneAsync(profile),
                context.Groups.InsertOneAsync(group)
            };

            await Task.WhenAll(insertTasks);

            var query = new GetGroupByIdQuery
            {
                Id = group.Id
            };

            var expectedResult = new GroupInfoWithOwner
            {
                Id = 1,
                Name = "test",
                Owner = new ProfileResponse
                {
                    Id= 1,
                    Email = "test@mail.com",
                    Name = "test"
                }
            };

            // Act
            var profileGroup = await sender.Send(query, CancellationToken.None);

            // Assert
            profileGroup.Should().BeEquivalentTo(expectedResult);
        }
    }
}
