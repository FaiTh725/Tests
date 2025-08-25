using FluentAssertions;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Domain.UnitTests.FeedbackReplies
{
    public class InitializeFeedbackReplyTests
    {
        [Fact]
        public void Initialize_WhenNullOrWhiteSpaceText_ShouldReturnFailedResult()
        {
            // Arrange
            var text = "   ";
            var ownerId = 1;
            var feedbackId = 1;

            // Act
            var feedbackReply = FeedbackReply.Initialize(text, feedbackId, ownerId);

            // Assert
            feedbackReply.IsFailure.Should().BeTrue();
            feedbackReply.Error.Should().Be("Text is null or white space or outside of " +
                    $"{FeedbackReplyValidator.MIN_REPLY_MESSAGE_LENGTH} {FeedbackReplyValidator.MAX_REPLY_MESSAGE_LENGTH}");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var text = "white space or outside";
            var ownerId = 1;
            var feedbackId = 1;
            var expectedCreatedTime = DateTime.UtcNow;

            // Act
            var feedbackReply = FeedbackReply.Initialize(text, feedbackId, ownerId);

            // Assert
            feedbackReply.IsSuccess.Should().BeTrue();
            feedbackReply.Value.Text.Should().Be(text);
            feedbackReply.Value.OwnerId.Should().Be(ownerId);
            feedbackReply.Value.FeedbackId.Should().Be(feedbackId);
            feedbackReply.Value.SendTime.Should()
                .BeCloseTo(expectedCreatedTime, TimeSpan.FromMicroseconds(1000));
            feedbackReply.Value.UpdateTime.Should()
                .BeCloseTo(expectedCreatedTime, TimeSpan.FromMicroseconds(1000));
            feedbackReply.Value.IsDeleted.Should().BeFalse();
            feedbackReply.Value.DeletedTime.Should().BeNull();
        }
    }
}
