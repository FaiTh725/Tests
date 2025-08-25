using FluentAssertions;
using TestRating.Domain.Entities;

namespace TestRating.Domain.UnitTests.FeedbackReplies
{
    public class DeleteFeedbackReplyTests
    {
        [Fact]
        public void Delete_WhenFeedbackReplyAlreadyDeleted_ShouldReturnFailedResult()
        {
            // Arrange
            var feedbackReply = FeedbackReply.Initialize("white space or outside", 1, 1);
            feedbackReply.Value.Delete();

            // Act
            var deleteResult = feedbackReply.Value.Delete();

            // Assert
            deleteResult.IsFailure.Should().BeTrue();
            deleteResult.Error.Should().Be("Reply has already deleted");
        }

        [Fact]
        public void Delete_WhenFeedbackReplyUnDeleted_ShouldReturnSuccessResult()
        {
            // Arrange
            var feedbackReply = FeedbackReply.Initialize("white space or outside", 1, 1);
            var expectedDeletedTime = DateTime.UtcNow;

            // Act
            var deleteResult = feedbackReply.Value.Delete();

            // Assert
            deleteResult.IsSuccess.Should().BeTrue();
            feedbackReply.Value.IsDeleted.Should().BeTrue();
            feedbackReply.Value.DeletedTime.Should()
                .BeCloseTo(expectedDeletedTime, TimeSpan.FromMicroseconds(1000));
        }
    }
}
