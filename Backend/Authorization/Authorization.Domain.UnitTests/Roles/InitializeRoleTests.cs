using Authorization.Domain.Entities;
using FluentAssertions;

namespace Authorization.Domain.UnitTests.Roles
{
    public class InitializeRoleTests
    {
        [Fact]
        public void Initialize_WhenEmptyOrWhiteSpaceRoleName_ShouldReturnFailedResult()
        {
            // Arrange
            var roleName = " ";

            // Act
            var user = Role.Initialize(roleName);

            // Assert
            user.IsFailure.Should().BeTrue();
            user.Error.Should().Be("Role is empty or null");
        }

        [Fact]
        public void Initialize_CorrectRoleName_ShouldReturnSuccessResult()
        {
            // Arrange
            var roleName = "User";

            // Act
            var user = Role.Initialize(roleName);

            // Assert
            user.IsSuccess.Should().BeTrue();
            user.Value.RoleName.Should().Be(roleName);
        }
    }
}
