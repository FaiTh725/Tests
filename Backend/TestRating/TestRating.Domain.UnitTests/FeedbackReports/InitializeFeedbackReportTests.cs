using FluentAssertions;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Domain.UnitTests.FeedbackReports
{
    public class InitializeFeedbackReportTests
    {
        [Fact]
        public void Initialize_WhenNullOrEmptyReportMessage_ShouldReturnFailedResult()
        {
            // Arrange
            var reportMessage = string.Empty;
            var feedbackId = 1;
            var profileId = 1;

            // Act
            var initializeResult = FeedbackReport.Initialize(reportMessage, feedbackId, profileId);

            // Assert
            initializeResult.IsFailure.Should().BeTrue();
            initializeResult.Error.Should().Be("Message is empty or outside of " +
                    $"{ReportValidator.MIN_REPORT_MESSAGE_LENGTH} - {ReportValidator.MAX_REPORT_MESSAGE_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenOutsideOfBoundsReportMessage_ShouldReturnFailedResult()
        {
            // Arrange
            var reportMessage = "d";
            var feedbackId = 1;
            var profileId = 1;

            // Act
            var initializeResult = FeedbackReport.Initialize(reportMessage, feedbackId, profileId);

            // Assert
            initializeResult.IsFailure.Should().BeTrue();
            initializeResult.Error.Should().Be("Message is empty or outside of " +
                    $"{ReportValidator.MIN_REPORT_MESSAGE_LENGTH} - {ReportValidator.MAX_REPORT_MESSAGE_LENGTH}");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var reportMessage = "Invalid photos";
            var feedbackId = 1;
            var profileId = 1;
            var expectedCreatedTime = DateTime.UtcNow;

            // Act
            var initializeResult = FeedbackReport.Initialize(reportMessage, feedbackId, profileId);

            // Assert
            initializeResult.IsSuccess.Should().BeTrue();
            initializeResult.Value.ReportMessage.Should().Be(reportMessage);
            initializeResult.Value.ReportedFeedbackId.Should().Be(feedbackId);
            initializeResult.Value.ReviewerId.Should().Be(profileId);
            initializeResult.Value.IsApproval.Should().BeNull();
            initializeResult.Value.CreatedTime.Should()
                .BeCloseTo(expectedCreatedTime, TimeSpan.FromMicroseconds(1000));
        }
    }
}
