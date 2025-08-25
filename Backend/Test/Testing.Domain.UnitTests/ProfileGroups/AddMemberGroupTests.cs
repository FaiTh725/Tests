using FluentAssertions;
using Test.Domain.Entities;

namespace Testing.Domain.UnitTests.ProfileGroups
{
    public class AddMemberGroupTests
    {
        [Fact]
        public void AddMember_WhenMemberAlreadyInGroup_ShouldReturnFailedResult()
        {
            // Arrange
            var groupMembers = new List<long> { 2 };
            var profileGroup = ProfileGroup.Initialize("Cowboys", 1, groupMembers);
            var newGroupMember = 2;

            // Act
            var addResult = profileGroup.Value.AddMember(newGroupMember);

            // Assert
            addResult.IsFailure.Should().BeTrue();
            addResult.Error.Should().Be("Member already in group");
        }

        [Fact]
        public void AddMember_WhenMemberArentInGroup_ShouldReturnSuccessResult()
        {
            // Arrange
            var groupMembers = new List<long> { 2 };
            var profileGroup = ProfileGroup.Initialize("Cowboys", 1, groupMembers);
            var newGroupMember = 3;
            var expectedProfileGroupMembers = new List<long> { 2, 3 };

            // Act
            var addResult = profileGroup.Value.AddMember(newGroupMember);

            // Assert
            addResult.IsSuccess.Should().BeTrue();
            profileGroup.Value.MembersId.Should()
                .BeEquivalentTo(expectedProfileGroupMembers);
        }
    }
}
