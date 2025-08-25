using FluentAssertions;
using Test.Domain.Entities;

namespace Testing.Domain.UnitTests.ProfileGroups
{
    public class DeleteMembersGroupTests
    {
        [Fact]
        public void DeleteMember_WhenMembersArentInGroup_ShouldReturnFailedResult()
        {
            // Arrange
            var groupMembers = new List<long> { 2 };
            var profileGroup = ProfileGroup.Initialize("Cowboys", 1, groupMembers);
            var membersToDelete = new List<long> { 3 };

            // Act
            var addResult = profileGroup.Value.DeleteMembers(membersToDelete);

            // Assert
            addResult.IsFailure.Should().BeTrue();
            addResult.Error.Should().Be("Some members arent in the group");
        }

        [Fact]
        public void DeleteMember_WhenMembersAreInGroup_ShouldReturnSuccessResult()
        {
            // Arrange
            var groupMembers = new List<long> { 2 };
            var profileGroup = ProfileGroup.Initialize("Cowboys", 1, groupMembers);
            var membersToDelete = new List<long> { 2 };
            var expectedMembersGroup = new List<long>();

            // Act
            var addResult = profileGroup.Value.DeleteMembers(membersToDelete);

            // Assert
            addResult.IsSuccess.Should().BeTrue();
            profileGroup.Value.MembersId.Should()
                .BeEquivalentTo(expectedMembersGroup);
        }
    }
}
