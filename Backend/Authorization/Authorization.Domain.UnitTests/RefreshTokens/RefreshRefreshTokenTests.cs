using Authorization.Domain.Entities;
using FluentAssertions;

namespace Authorization.Domain.UnitTests.RefreshTokens
{
    public class RefreshRefreshTokenTests
    {
        private readonly User user;
        private readonly RefreshToken refreshToken;

        public RefreshRefreshTokenTests()
        {
            user = User.Initialize("Faith", 
                "sasha.zelenukho@mail.ru", 
                "787ca7e12a2fa991cea5051a64b49d0c", 
                "User").Value;

            refreshToken = RefreshToken.Initialize("787ca7e12a2fa99", 
                user, 
                DateTime.UtcNow.AddDays(15)).Value;
        }

        [Fact]
        public void Refresh_WhenExpireDateTimePointsToThePast_ShouldReturnFailedResult()
        {
            // Arrange
            var token = "787ca7e12a2fa99";
            var expireOnUtc = DateTime.UtcNow.AddDays(-15);

            // Act
            var refreshResult = refreshToken.Refresh(token, expireOnUtc);

            // Assert
            refreshResult.IsFailure.Should().BeTrue();
            refreshResult.Error.Should().Be("Expire time points on the past");
        }

        [Fact]
        public void Refresh_WhenTokenIsNullOrWhiteSpace_ShouldReturnFailedResult()
        {
            // Arrange
            var token = " ";
            var expireOnUtc = DateTime.UtcNow.AddDays(15);

            // Act
            var refreshResult = refreshToken.Refresh(token, expireOnUtc);

            // Assert
            refreshResult.IsFailure.Should().BeTrue();
            refreshResult.Error.Should().Be("Token is empty");
        }

        [Fact]
        public void Refresh_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var token = "e75a4f4a0c85";
            var expireOnUtc = DateTime.UtcNow.AddDays(15);

            // Act
            var refreshResult = refreshToken.Refresh(token, expireOnUtc);

            // Assert
            refreshResult.IsSuccess.Should().BeTrue();
            refreshToken.Token.Should().Be(token);
            refreshToken.ExpireOn.Should().Be(expireOnUtc);
        }
    }
}
