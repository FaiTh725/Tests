using FluentAssertions;
using Test.Domain.Entities;
using Test.Domain.Validators;

namespace Testing.Domain.UnitTests.TestSessions
{
    public class CloseTestSessionTests
    {
        [Fact]
        public void CloseSession_IfCloseAlreadyClosedSession_ShouldReturnFailedResult()
        {
            // Arrange
            var testSession = TestSession.Initialize(1, 1);
            var percent = 12;
            testSession.Value.CloseSession(percent);

            // Act
            var closeSessionResult = testSession.Value.CloseSession(percent);

            // Assert
            closeSessionResult.IsFailure.Should().BeTrue();
            closeSessionResult.Error.Should().Be("Session has already ended");
        }

        [Fact]
        public void CloseSession_WhenOutSideOfRangePercent_ShouldReturnFailedResult()
        {
            // Arrange
            var testSession = TestSession.Initialize(1, 1);
            var percent = 112;

            // Act
            var closeSessionResult = testSession.Value.CloseSession(percent);

            // Assert
            closeSessionResult.IsFailure.Should().BeTrue();
            closeSessionResult.Error.Should().Be("Percent outside from range " +
                    $"[{TestSessionValidator.MIN_PERCENT}, {TestSessionValidator.MAX_PERCENT}]");
        }

        [Fact]
        public void CloseSession_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var testSession = TestSession.Initialize(1, 1);
            var percent = 50;
            var expectedEndTime = DateTime.UtcNow;

            // Act
            var closeSessionResult = testSession.Value.CloseSession(percent);

            // Assert
            closeSessionResult.IsSuccess.Should().BeTrue();
            testSession.Value.Percent.Should().Be(percent);
            testSession.Value.IsEnded.Should().BeTrue();
            testSession.Value.EndTime.Should().BeCloseTo(expectedEndTime, TimeSpan.FromMicroseconds(1000));
        }
    }
}
