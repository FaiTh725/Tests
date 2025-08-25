using FluentAssertions;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Domain.UnitTests.FeedbackReplies
{
    public class UpdateFeedbackReplyTests
    {
        [Fact]
        public void ChangeReply_WhenOutsideFromLengthText_ShouldReturnFailedResult()
        {
            // Arrange
            var feedbackReply = FeedbackReply.Initialize("white space or outside", 1, 1);
            var text = "d";

            // Act
            var changeResult = feedbackReply.Value.ChangeReply(text);

            // Assert
            changeResult.IsFailure.Should().BeTrue();
            changeResult.Error.Should().Be("Text is null or white space or outside of " +
                    $"{FeedbackReplyValidator.MIN_REPLY_MESSAGE_LENGTH} {FeedbackReplyValidator.MAX_REPLY_MESSAGE_LENGTH}");
        }

        [Fact]
        public void ChangeReply_WhenEmptyOrWhiteSpaceText_ShouldReturnFailedResult()
        {
            // Arrange
            var feedbackReply = FeedbackReply.Initialize("white space or outside", 1, 1);
            var text = string.Empty;

            // Act
            var changeResult = feedbackReply.Value.ChangeReply(text);

            // Assert
            changeResult.IsFailure.Should().BeTrue();
            changeResult.Error.Should().Be("Text is null or white space or outside of " +
                    $"{FeedbackReplyValidator.MIN_REPLY_MESSAGE_LENGTH} {FeedbackReplyValidator.MAX_REPLY_MESSAGE_LENGTH}");
        }

        [Fact]
        public void ChangeReply_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var feedbackReply = FeedbackReply.Initialize("white space or outside", 1, 1);
            var text = "outside";
            var updatedExpectedTime = DateTime.UtcNow;

            // Act
            var changeResult = feedbackReply.Value.ChangeReply(text);

            // Assert
            changeResult.IsSuccess.Should().BeTrue();
            feedbackReply.Value.Text.Should().Be(text);
            feedbackReply.Value.UpdateTime.Should()
                .BeCloseTo(updatedExpectedTime, TimeSpan.FromMicroseconds(1000));
        }
    }
}
