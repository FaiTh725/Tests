using FluentAssertions;
using TestRating.Domain.Entities;

namespace TestRating.Domain.UnitTests.FeedbackReviews
{
    public class InitializeFeedbackReviewTests
    {
        [Fact]
        public void Initialize_InitializeReview_ShoudReturnCorrectInstance()
        {
            // Arrange
            var isPositive = true;
            var ownerId = 1;
            var reviewedFeedbackId = 1;

            // Act
            var feedbackReview = FeedbackReview.Initialize(isPositive, ownerId, reviewedFeedbackId);

            // Assert
            feedbackReview.IsSuccess.Should().BeTrue();
            feedbackReview.Value.IsPositive.Should().Be(isPositive);
            feedbackReview.Value.OwnerId.Should().Be(ownerId);
            feedbackReview.Value.ReviewedFeedbackId.Should().Be(reviewedFeedbackId);
        }
    }
}
