using FluentAssertions;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Domain.UnitTests.Feedbacks
{
    public class UpdateFeedbackTests
    {
        [Fact]
        public void ChangeFeedback_WhenOutsideFromBoundsRating_ShouldReturnFailedResult()
        {
            // Arrange
            var feedback = Feedback.Initialize("qwertertujmngfffg", 1, 5, 1);
            var rating = 12;
            var text = "qwertertujmngfffg";

            // Act
            var changeResult = feedback.Value.ChangeFeedback(text, rating);

            // Assert
            changeResult.IsFailure.Should().BeTrue();
            changeResult.Error.Should().Be("Invalid values to update feedback - " +
                    "Rating should be in range " +
                    $"from {FeedbackValidator.MIN_FEEDBACK_RATING} to {FeedbackValidator.MAX_FEEDBACK_RATING}");
        }

        [Fact]
        public void ChangeFeedback_WhenOutsideFromLengthText_ShouldReturnFailedResult()
        {
            // Arrange
            var feedback = Feedback.Initialize("qwertertujmngfffg", 1, 5, 1);
            var rating = 3;
            var text = "d";

            // Act
            var changeResult = feedback.Value.ChangeFeedback(text, rating);

            // Assert
            changeResult.IsFailure.Should().BeTrue();
            changeResult.Error.Should().Be("Invalid values to update feedback - " +
                    "Text is empty or white space or outside of " +
                    $"{FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH} - {FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH}");
        }

        [Fact]
        public void ChangeFeedback_WhenEmptyOrWhiteSpaceText_ShouldReturnFailedResult()
        {
            // Arrange
            var feedback = Feedback.Initialize("qwertertujmngfffg", 1, 5, 1);
            var rating = 3;
            var text = string.Empty;

            // Act
            var changeResult = feedback.Value.ChangeFeedback(text, rating);

            // Assert
            changeResult.IsFailure.Should().BeTrue();
            changeResult.Error.Should().Be("Invalid values to update feedback - " +
                    "Text is empty or white space or outside of " +
                    $"{FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH} - {FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH}");
        }

        [Fact]
        public void ChangeFeedback_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var feedback = Feedback.Initialize("qwertertujmngfffg", 1, 5, 1);
            var rating = 3;
            var text = "fsdffgerw";
            var expectedUpdateTime = DateTime.UtcNow;

            // Act
            var changeResult = feedback.Value.ChangeFeedback(text, rating);

            // Assert
            changeResult.IsSuccess.Should().BeTrue();
            feedback.Value.Rating.Should().Be(rating);
            feedback.Value.Text.Should().Be(text);
            feedback.Value.UpdateTime.Should()
                .BeCloseTo(expectedUpdateTime, TimeSpan.FromMicroseconds(1000));
        }
    }
}
