using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Queries.FeedbackReviewEntity.GetFeedbackReview;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReviews.Queries
{
    public class GetFeedbackReviewQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IFeedbackReviewRepository> reviewRepository;
        
        private readonly GetFeedbackReviewHandler handler;

        public GetFeedbackReviewQueryHandlerTests()
        {
            unitOfWork = new();
            reviewRepository = new();

            handler = new GetFeedbackReviewHandler(unitOfWork.Object);

            unitOfWork.Setup(x => x.ReviewRepository)
                .Returns(reviewRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenReviewDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetFeedbackReviewQuery 
            { 
                Id = 1 
            };

            reviewRepository.Setup(x => x
                .GetReview(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReview);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);
        
            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Review doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenReviewExist_ShouldReturnReview()
        {
            // Arrange
            var query = new GetFeedbackReviewQuery
            {
                Id = 1
            };

            var existedReview = FeedbackReview.Initialize(true, 1, 1).Value;
            // set Id
            var type = typeof(FeedbackReview);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedReview, [1]);

            reviewRepository.Setup(x => x
                .GetReview(
                    query.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReview);

            // Act
            var review = await handler.Handle(query, CancellationToken.None);

            // Assert
            review.Id.Should().Be(existedReview.Id);
            review.IsPositive.Should().Be(existedReview.IsPositive);
            review.OwnerId.Should().Be(existedReview.OwnerId);
            review.FeedbackId.Should().Be(existedReview.ReviewedFeedbackId);
        }
    }
}
