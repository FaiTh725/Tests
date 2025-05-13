using FluentAssertions;
using TestRating.Domain.Entities;

namespace TestRating.Domain.UnitTests.FeedbackReports
{
    public class ReviewFeedbackReportTests
    {
        [Fact]
        public void Review_ShouldChangeReportStatus()
        {
            // Arrange
            var report = FeedbackReport.Initialize("Incorrect Photos", 1, 1);
            var expectedApprove = true;

            // Act
            report.Value.ReviewReport(true);

            // Assert
            report.Value.IsApproval.Should().Be(expectedApprove);
        }
    }
}
