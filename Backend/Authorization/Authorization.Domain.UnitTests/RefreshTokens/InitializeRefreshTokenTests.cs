using Authorization.Domain.Entities;
using FluentAssertions;

namespace Authorization.Domain.UnitTests.RefreshTokens
{
    public class InitializeRefreshTokenTests
    {
        [Fact]
        public void Initialize_WhenEmptyOrWhiteSpaceToken_ShouldReturnFailedResult()
        {
            // Arrange
            var token = string.Empty;
            var user = User.Initialize("Faith", "sasha.zelenukho@mail.ru", "787ca7e12a2fa991cea5051a64b49d0c", "User");
            var expireOnUtc = DateTime.UtcNow.AddDays(15);

            // Act
            var refreshToken = RefreshToken.Initialize(token, user.Value, expireOnUtc);

            // Assert
            refreshToken.IsFailure.Should().BeTrue();
            refreshToken.Error.Should().Be("Token is empty or null");
        }
        
        [Fact]
        public void Initialize_WhenUserIsNull_ShouldReturnFailedResult()
        {
            // Arrange
            var token = "787ca7e12a2fa99";
            User user = null;
            var expireOnUtc = DateTime.UtcNow.AddDays(15);

            // Act
            var refreshToken = RefreshToken.Initialize(token, user, expireOnUtc);

            // Assert
            refreshToken.IsFailure.Should().BeTrue();
            refreshToken.Error.Should().Be("User is null");
        }

        [Fact]
        public void Initialize_WhenExpireDateTimePointsToThePast_ShouldReturnFailedResult()
        {
            // Arrange
            var token = "787ca7e12a2fa99";
            var user = User.Initialize("Faith", "sasha.zelenukho@mail.ru", "787ca7e12a2fa991cea5051a64b49d0c", "User");
            var expireOnUtc = DateTime.UtcNow.AddDays(-15);

            // Act
            var refreshToken = RefreshToken.Initialize(token, user.Value, expireOnUtc);

            // Assert
            refreshToken.IsFailure.Should().BeTrue();
            refreshToken.Error.Should().Be("Expire time points on the past");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var token = "787ca7e12a2fa99";
            var user = User.Initialize("Faith", "sasha.zelenukho@mail.ru", "787ca7e12a2fa991cea5051a64b49d0c", "User");
            var expireOnUtc = DateTime.UtcNow.AddDays(15);

            // Act
            var refreshToken = RefreshToken.Initialize(token, user.Value, expireOnUtc);

            // Assert
            refreshToken.IsSuccess.Should().BeTrue();
            refreshToken.Value.User.Should().BeEquivalentTo(user.Value);
            refreshToken.Value.Token.Should().Be(token);
            refreshToken.Value.ExpireOn.Should().Be(expireOnUtc);
        }
    }
}
