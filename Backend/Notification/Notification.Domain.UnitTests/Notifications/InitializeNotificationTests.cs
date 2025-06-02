using FluentAssertions;
using Notification.Domain.Entities;
using Notification.Domain.Validators;

namespace Notification.Domain.UnitTests.Notifications
{
    public class InitializeNotificationTests
    {
        [Fact]
        public void Initialize_WhenEmailIsEmpty_ShouldReturnFailureResult()
        {
            // Arrange
            var email = string.Empty;
            var title = "title";
            var message = "message";

            // Act
            var initializeResult = NotificationEntity.Initialize(email, title, message);

            // Assert
            initializeResult.IsFailure.Should().BeTrue();
            initializeResult.Error.Should().Be("Email is empty or " +
                    "doesnt contains @ or dot after @");
        }

        [Fact]
        public void Initialize_WhenEmailDoesntContainsAt_ShouldReturnFailureResult()
        {
            // Arrange
            var email = "testenail.com";
            var title = "title";
            var message = "message";

            // Act
            var initializeResult = NotificationEntity.Initialize(email, title, message);

            // Assert
            initializeResult.IsFailure.Should().BeTrue();
            initializeResult.Error.Should().Be("Email is empty or " +
                    "doesnt contains @ or dot after @");
        }

        [Fact]
        public void Initialize_WhenEmailDoesntContainsDotAfterAt_ShouldReturnFailureResult()
        {
            // Arrange
            var email = "testenail.c@om";
            var title = "title";
            var message = "message";

            // Act
            var initializeResult = NotificationEntity.Initialize(email, title, message);

            // Assert
            initializeResult.IsFailure.Should().BeTrue();
            initializeResult.Error.Should().Be("Email is empty or " +
                    "doesnt contains @ or dot after @");
        }

        [Fact]
        public void Initialize_WhenTitleOutOfBounds_ShouldReturnFailureResult()
        {
            // Arrange
            var email = "test@mail.com";
            var title = "1";
            var message = "message";

            // Act
            var initializeResult = NotificationEntity.Initialize(email, title, message);

            // Assert
            initializeResult.IsFailure.Should().BeTrue();
            initializeResult.Error.Should().Be("Title is empty or length out of bounds " +
                    $"{NotificationValidator.MIN_TITLE_LENGTH} - {NotificationValidator.MAX_TITLE_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenMessageOutOfBounds_ShouldReturnFailureResult()
        {
            // Arrange
            var email = "test@mail.com";
            var title = "title";
            var message = "m";

            // Act
            var initializeResult = NotificationEntity.Initialize(email, title, message);

            // Assert
            initializeResult.IsFailure.Should().BeTrue();
            initializeResult.Error.Should().Be("Message is empty or length out of bounds " +
                    $"{NotificationValidator.MIN_MESSAGE_LENGTH} - {NotificationValidator.MAX_MESSAGE_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenParamsIsCorrect_ShouldReturnSuccessResult()
        {
            // Arrange
            var email = "test@mail.com";
            var title = "title";
            var message = "message";

            var expectedSendTime = DateTime.UtcNow;

            // Act
            var initializeResult = NotificationEntity.Initialize(email, title, message);

            // Assert
            initializeResult.IsSuccess.Should().BeTrue();
            initializeResult.Value.Message.Should().Be(message);
            initializeResult.Value.Title.Should().Be(title);
            initializeResult.Value.UserEmail.Should().Be(email);
            initializeResult.Value.IsRead.Should().BeFalse();
            initializeResult.Value.SendTime.Should().BeCloseTo(expectedSendTime, TimeSpan.FromMilliseconds(100));
        }
    }
}
