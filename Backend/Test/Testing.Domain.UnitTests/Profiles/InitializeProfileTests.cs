using FluentAssertions;
using Test.Domain.Entities;

namespace Testing.Domain.UnitTests.Profiles
{
    public class InitializeProfileTests
    {
        [Fact]
        public void Initialize_WhenEmptyOrWhiteSpaceName_ShouldReturnFailedResult()
        {
            // Arrange
            var email = "sasha.zelenukho@mail.ru";
            var profileName = string.Empty;

            // Act
            var profile = Profile.Initialize(profileName, email);

            // Assert
            profile.IsFailure.Should().BeTrue();
            profile.Error.Should().Be("Name is null or white space");
        }

        [Fact]
        public void Initialize_WhenEmailDoesntContainAt_ShouldReturnFailedResult()
        {
            // Arrange
            var email = "sasha.zelenukhomail.ru";
            var profileName = "Faith";

            // Act
            var profile = Profile.Initialize(profileName, email);

            // Assert
            profile.IsFailure.Should().BeTrue();
            profile.Error.Should().Be("Email is empty or invalid signature");
        }

        [Fact]
        public void Initialize_WhenEmailDoesntContainDotAfterAt_ShouldReturnFailedResult()
        {
            // Arrange
            var email = "sasha.zelenukho@mailru";
            var profileName = "Faith";

            // Act
            var profile = Profile.Initialize(profileName, email);

            // Assert
            profile.IsFailure.Should().BeTrue();
            profile.Error.Should().Be("Email is empty or invalid signature");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var email = "sasha.zelenukho@mail.ru";
            var profileName = "Faith";

            // Act
            var profile = Profile.Initialize(profileName, email);

            // Assert
            profile.IsSuccess.Should().BeTrue();
            profile.Value.Email.Should().Be(email);
            profile.Value.Name.Should().Be(profileName);
        }
    }
}
