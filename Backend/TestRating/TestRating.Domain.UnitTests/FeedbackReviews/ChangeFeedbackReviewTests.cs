using FluentAssertions;
using TestRating.Domain.Entities;

namespace TestRating.Domain.UnitTests.FeedbackReviews
{
    public class ChangeFeedbackReviewTests
    {
        [Fact]
        public void ChangeFeedbackReview_ChnageReviewFromParameter()
        {
            // Arrange
            var feedbackReview = FeedbackReview.Initialize(true, 1, 1);
            var expectedFeedbackReview = true;

            // Act
            feedbackReview.Value.ChangeReview(true);

            // Assert
            feedbackReview.Value.IsPositive.Should().Be(expectedFeedbackReview);
        }
    }
}
