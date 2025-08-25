using FluentAssertions;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Domain.UnitTests.Feedbacks
{
    public class InitializeFeedbackTests
    {
        [Fact]
        public void Initialize_WhenNullOrWhiteSpaceText_ShouldReturnFailedResult()
        {
            // Arrange
            var text = "   ";
            var testId = 1;
            var rating = 5;
            var ownerId = 1;

            // Act
            var feedback = Feedback.Initialize(text, testId, rating, ownerId);

            // Assert
            feedback.IsFailure.Should().BeTrue();
            feedback.Error.Should().Be("Text is empty or white space or outside of " +
                    $"{FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH} - {FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenOutsideFromLengthText_ShouldReturnFailedResult()
        {
            // Arrange
            var text = "d";
            var testId = 1;
            var rating = 5;
            var ownerId = 1;

            // Act
            var feedback = Feedback.Initialize(text, testId, rating, ownerId);

            // Assert
            feedback.IsFailure.Should().BeTrue();
            feedback.Error.Should().Be("Text is empty or white space or outside of " +
                    $"{FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH} - {FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenOutsideFromBoundsRating_ShouldReturnFailedResult()
        {
            // Arrange
            var text = "qwertertujmngfffgdf23556f tfwe";
            var testId = 1;
            var rating = 12;
            var ownerId = 1;

            // Act
            var feedback = Feedback.Initialize(text, testId, rating, ownerId);

            // Assert
            feedback.IsFailure.Should().BeTrue();
            feedback.Error.Should().Be("Rating should be in range " +
                    $"from {FeedbackValidator.MIN_FEEDBACK_RATING} to {FeedbackValidator.MAX_FEEDBACK_RATING}");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var text = "qwertertujmngfffgdf23556f tfwe";
            var testId = 1;
            var rating = 5;
            var ownerId = 1;
            var expectedTime = DateTime.UtcNow;

            // Act
            var feedback = Feedback.Initialize(text, testId, rating, ownerId);

            // Assert
            feedback.IsSuccess.Should().BeTrue();
            feedback.Value.Text.Should().Be(text);
            feedback.Value.TestId.Should().Be(testId);
            feedback.Value.Rating.Should().Be(rating);
            feedback.Value.OwnerId.Should().Be(ownerId);
            feedback.Value.SendTime.Should()
                .BeCloseTo(expectedTime, TimeSpan.FromMicroseconds(1000));
            feedback.Value.UpdateTime.Should()
                .BeCloseTo(expectedTime, TimeSpan.FromMicroseconds(1000));
            feedback.Value.IsDeleted.Should().BeFalse();
            feedback.Value.DeletedTime.Should().BeNull();
        }
    }
}
