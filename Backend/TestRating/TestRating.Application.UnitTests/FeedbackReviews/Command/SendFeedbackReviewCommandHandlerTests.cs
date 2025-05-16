using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.FeedbackReviewEntity.SendFeedbackReview;
using TestRating.Application.Queries.FeedbackReviewEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReviews.Command
{
    public class SendFeedbackReviewCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;
        private readonly Mock<IFeedbackReviewRepository> reviewRepositoryMock;

        private readonly SendFeedbackReviewHandler handler;

        public SendFeedbackReviewCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();
            feedbackRepositoryMock = new();
            reviewRepositoryMock = new();

            handler = new SendFeedbackReviewHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ReviewRepository)
                .Returns(reviewRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendFeedbackReviewCommand()
            {
                FeedbackId = 1,
                IsPositive = true,
                ProfileId = 1
            };

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.ProfileId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendFeedbackReviewCommand()
            {
                FeedbackId = 1,
                IsPositive = true,
                ProfileId = 1
            };
            var existedProfile = Profile.Initialize("zelenukho725@gmail.com", "FaiTh").Value;

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Feedback);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileDidntSendReview_ShouldAddReview()
        {
            // Arrange
            var command = new SendFeedbackReviewCommand()
            {
                FeedbackId = 1,
                IsPositive = true,
                ProfileId = 1
            };
            var existedProfile = Profile.Initialize("zelenukho725@gmail.com", "FaiTh").Value;
            var existedFeedback = Feedback.Initialize("some text here", 1, 5, 1).Value;

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            reviewRepositoryMock.Setup(x => x
                .GetFeedbackReviewByCriteria(
                    It.IsAny<ReviewProfileToFeedbackSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReview);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            reviewRepositoryMock.Verify(x => x
                .AddReview(
                    It.IsAny<FeedbackReview>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            reviewRepositoryMock.Verify(x => x
                .UpdateReview(
                    It.IsAny<long>(),
                    It.IsAny<FeedbackReview>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            reviewRepositoryMock.Verify(x => x
                .DeleteReview(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenProfileSentSimilarReview_ShouldDeletePastReview()
        {
            // Arrange
            var command = new SendFeedbackReviewCommand()
            {
                FeedbackId = 1,
                IsPositive = true,
                ProfileId = 1
            };
            var existedProfile = Profile.Initialize("zelenukho725@gmail.com", "FaiTh").Value;
            var existedFeedback = Feedback.Initialize("some text here", 1, 5, 1).Value;
            var existedReview = FeedbackReview.Initialize(true, 1, 1).Value;

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            reviewRepositoryMock.Setup(x => x
                .GetFeedbackReviewByCriteria(
                    It.IsAny<ReviewProfileToFeedbackSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReview);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            reviewRepositoryMock.Verify(x => x
                .AddReview(
                    It.IsAny<FeedbackReview>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            reviewRepositoryMock.Verify(x => x
                .UpdateReview(
                    It.IsAny<long>(),
                    It.IsAny<FeedbackReview>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            reviewRepositoryMock.Verify(x => x
                .DeleteReview(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProfileSentAnotherReview_ShouldUpdatePastReview()
        {
            // Arrange
            var command = new SendFeedbackReviewCommand()
            {
                FeedbackId = 1,
                IsPositive = false,
                ProfileId = 1
            };
            var existedProfile = Profile.Initialize("zelenukho725@gmail.com", "FaiTh").Value;
            var existedFeedback = Feedback.Initialize("some text here", 1, 5, 1).Value;
            var existedReview = FeedbackReview.Initialize(true, 1, 1).Value;

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            reviewRepositoryMock.Setup(x => x
                .GetFeedbackReviewByCriteria(
                    It.IsAny<ReviewProfileToFeedbackSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReview);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            existedReview.IsPositive.Should().Be(command.IsPositive);

            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            reviewRepositoryMock.Verify(x => x
                .AddReview(
                    It.IsAny<FeedbackReview>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            reviewRepositoryMock.Verify(x => x
                .UpdateReview(
                    It.IsAny<long>(),
                    It.IsAny<FeedbackReview>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            reviewRepositoryMock.Verify(x => x
                .DeleteReview(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
