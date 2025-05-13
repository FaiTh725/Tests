using Authorization.Domain.Entities;
using FluentAssertions;

namespace Authorization.Domain.UnitTests.Users
{
    public class InitializeUserTests
    {
        [Fact]
        public void Initialize_WhenEmptyOrWhiteSpaceFullName_ShouldReturnFailedResult()
        {
            // Arrange
            var email = "sasha.zelenukho@mail.ru";
            var userName = string.Empty;
            var passwordHash = "787ca7e12a2fa991cea5051a64b49d0c";
            var roleName = "User";

            // Act
            var user = User.Initialize(userName, email, passwordHash, roleName);

            // Assert
            user.IsFailure.Should().BeTrue();
            user.Error.Should().Be("UserName is empty or null");
        }

        [Fact]
        public void Initialize_WhenEmptyOrWhiteSpacePasswordHash_ShouldReturnFailedResult()
        {
            // Arrange
            var email = "sasha.zelenukho@mail.ru";
            var userName = "Faith";
            var passwordHash = string.Empty;
            var roleName = "User";

            // Act
            var user = User.Initialize(userName, email, passwordHash, roleName);

            // Assert
            user.IsFailure.Should().BeTrue();
            user.Error.Should().Be("PasswordHash is empty or null");
        }

        [Fact]
        public void Initialize_WhenEmptyOrWhiteSpaceRoleName_ShouldReturnFailedResult()
        {
            // Arrange
            var email = "sasha.zelenukho@mail.ru";
            var userName = "Faith";
            var passwordHash = "787ca7e12a2fa991cea5051a64b49d0c";
            var roleName = "  ";

            // Act
            var user = User.Initialize(userName, email, passwordHash, roleName);

            // Assert
            user.IsFailure.Should().BeTrue();
            user.Error.Should().Be("Role is empty or null");
        }

        [Fact]
        public void Initialize_WhenEmailDoesntContainAt_ShouldReturnFailedResult()
        {
            // Arrange
            var email = "sasha.zelenukhomail.ru";
            var userName = "Faith";
            var passwordHash = "787ca7e12a2fa991cea5051a64b49d0c";
            var roleName = "User";

            // Act
            var user = User.Initialize(userName, email, passwordHash, roleName);

            // Assert
            user.IsFailure.Should().BeTrue();
            user.Error.Should().Be("Email is invalid, must contain @ and a dot after it");
        }

        [Fact]
        public void Initialize_WhenEmailDoesntContainsDotAfterAt_ShouldReturnFailedResult()
        {
            // Arrange
            var email = "sasha.zelenukho@mailru";
            var userName = "Faith";
            var passwordHash = "787ca7e12a2fa991cea5051a64b49d0c";
            var roleName = "User";

            // Act
            var user = User.Initialize(userName, email, passwordHash, roleName);

            // Assert
            user.IsFailure.Should().BeTrue();
            user.Error.Should().Be("Email is invalid, must contain @ and a dot after it");
        }

        [Fact]
        public void Initialize_CorrectUserParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var email = "sasha.zelenukho@mail.ru";
            var userName = "Faith";
            var passwordHash = "787ca7e12a2fa991cea5051a64b49d0c";
            var roleName = "User";

            // Act
            var user = User.Initialize(userName, email, passwordHash, roleName);

            // Assert
            user.IsSuccess.Should().BeTrue();
            user.Value.Email.Should().Be(email);
            user.Value.UserName.Should().Be(userName);
            user.Value.PasswordHash.Should().Be(passwordHash);
            user.Value.RoleId.Should().Be(roleName);
        }
    }
}
