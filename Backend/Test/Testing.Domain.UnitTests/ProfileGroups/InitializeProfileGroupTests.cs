using FluentAssertions;
using Test.Domain.Entities;
using Test.Domain.Validators;

namespace Testing.Domain.UnitTests.ProfileGroups
{
    public class InitializeProfileGroupTests
    {
        [Fact]
        public void Initialize_WhenIsNullOrWhiteSpaceGroupName_ShouldReturnFailedResult()
        {
            // Arrange
            var groupName = " ";
            var profileId = 1;

            // Act
            var profileGroup = ProfileGroup.Initialize(groupName, profileId);

            // Assert
            profileGroup.IsFailure.Should().BeTrue();
            profileGroup.Error.Should().Be("Name group is null or length great " +
                    $"{ProfileGroupValidator.MAX_GROUP_NAME_LENGTH} or less {ProfileGroupValidator.MIN_GROUP_NAME_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenOutOfBoundsLengthGroupName_ShouldReturnFailedResult()
        {
            // Arrange
            var groupName = "q";
            var profileId = 1;

            // Act
            var profileGroup = ProfileGroup.Initialize(groupName, profileId);

            // Assert
            profileGroup.IsFailure.Should().BeTrue();
            profileGroup.Error.Should().Be("Name group is null or length great " +
                    $"{ProfileGroupValidator.MAX_GROUP_NAME_LENGTH} or less {ProfileGroupValidator.MIN_GROUP_NAME_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenIsNullListMembers_ShouldReturnFailedResult()
        {
            // Arrange
            var groupName = "Cowboys";
            var profileId = 1;
            List<long> listMembersId = null;

            // Act
            var profileGroup = ProfileGroup.Initialize(groupName, profileId, listMembersId);

            // Assert
            profileGroup.IsFailure.Should().BeTrue();
            profileGroup.Error.Should().Be("MembersId list is null");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var groupName = "Cowboys";
            var profileId = 1;
            List<long> listMembersId = new List<long>
            {
                2, 3, 4
            };

            // Act
            var profileGroup = ProfileGroup.Initialize(groupName, profileId, listMembersId);

            // Assert
            profileGroup.IsSuccess.Should().BeTrue();
            profileGroup.Value.GroupName.Should().Be(groupName);
            profileGroup.Value.OwnerId.Should().Be(profileId);
            profileGroup.Value.MembersId.Should().BeEquivalentTo(listMembersId);
        }
    }
}
