using FluentAssertions;
using TestRating.Domain.Entities;
using TestRating.Domain.Validators;

namespace TestRating.Domain.UnitTests.Feedbacks
{
    public class DeleteFeedbackTests
    {
        [Fact]
        public void Delete_WhenFeedbackAlreadyDeleted_ShouldReturnFailedResult()
        {
            // Arrange
            var feedback = Feedback.Initialize("qwertertujmngfffg", 1, 5, 1);
            feedback.Value.Delete();

            // Act
            var deleteResult = feedback.Value.Delete();

            // Assert
            deleteResult.IsFailure.Should().BeTrue();
            deleteResult.Error.Should().Be("Feedback has already deleted");
        }

        [Fact]
        public void Delete_WhenFeedbackUnDeleted_ShouldReturnSuccessResult()
        {
            // Arrange
            var feedback = Feedback.Initialize("qwertertujmngfffg", 1, 5, 1);
            var expectedDeletedTime = DateTime.UtcNow;

            // Act
            var deleteResult = feedback.Value.Delete();

            // Assert
            deleteResult.IsSuccess.Should().BeTrue();
            feedback.Value.IsDeleted.Should().BeTrue();   
            feedback.Value.DeletedTime.Should()
                .BeCloseTo(expectedDeletedTime, TimeSpan.FromMicroseconds(1000));
        }
    }
}
