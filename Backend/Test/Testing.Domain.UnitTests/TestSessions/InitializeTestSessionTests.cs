using FluentAssertions;
using Test.Domain.Entities;
using Test.Domain.Validators;

namespace Testing.Domain.UnitTests.TestSessions
{
    public class InitializeTestSessionTests
    {
        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var testId = 1;
            var profileId = 1;
            var expectedStartTime = DateTime.UtcNow;

            // Act
            var testSession = TestSession.Initialize(testId, profileId);

            // Assert
            testSession.IsSuccess.Should().BeTrue();
            testSession.Value.TestId.Should().Be(testId);
            testSession.Value.ProfileId.Should().Be(profileId);
            testSession.Value.IsEnded.Should().BeFalse();
            testSession.Value.EndTime.Should().BeNull();
            testSession.Value.StartTime.Should()
                .BeCloseTo(expectedStartTime, TimeSpan.FromMicroseconds(1000));
        }
    }
}
