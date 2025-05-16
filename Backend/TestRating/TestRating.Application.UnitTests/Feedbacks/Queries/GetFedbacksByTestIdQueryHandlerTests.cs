using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using System.Data;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Feedback;
using TestRating.Application.Contacts.Pagination;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestId;
using TestRating.Application.Queries.FeedbackEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Feedbacks.Queries
{
    public class GetFedbacksByTestIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;
        private readonly Mock<ITestExternalService> externalServiceMock;
        private readonly Mock<IBlobService> blobServiceMock;

        private readonly GetFeedbacksByTestIdHandler handler;

        public GetFedbacksByTestIdQueryHandlerTests()
        {
            unitOfWorkMock = new();
            feedbackRepositoryMock = new();
            externalServiceMock = new();
            blobServiceMock = new();

            handler = new GetFeedbacksByTestIdHandler(
                unitOfWorkMock.Object, 
                externalServiceMock.Object, 
                blobServiceMock.Object);

            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new GetFeedbacksByTestIdQuery 
            { 
                Page = 1,
                PageSize = 12,
                TestId = 1
            };

            externalServiceMock.Setup(x => x
                .TestIsExists(
                    command.TestId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Test doesnt exist");
        }

        [Theory]
        [InlineData(1, 12)]
        [InlineData(2, 2)]
        public async Task Handle_WhenTestExists_ShouldReturnTestFeedbacks(int page, int pageSize)
        {
            // Arrange
            var command = new GetFeedbacksByTestIdQuery
            {
                Page = page,
                PageSize = pageSize,
                TestId = 1
            };
            var existedOwner = Profile.Initialize("test@mail.ru", "test").Value;
            // set id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedOwner, [1]);
            var existedFeedback = Feedback.Initialize("some text here", 1, 5, 1).Value;
            // set owner
            type = typeof(Feedback);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [1]);
            // set owner
            type = typeof(Feedback);
            property= type.GetProperty("Owner");   
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [existedOwner]);
            // set createdTime
            property = type.GetProperty("SendTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [new DateTime(2025, 5, 10)]);
            // set updatedTime
            property = type.GetProperty("UpdateTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [new DateTime(2025, 5, 10)]);

            var feedbacksFromDb = new List<Feedback> 
            { 
                existedFeedback
            };
            var expectedFeedbacks = new List<FeedbackWithReviewsResponse>() 
            { 
                new FeedbackWithReviewsResponse
                {
                    Profile = new BaseProfileResponse
                    {
                        Id = 1,
                        Email = "test@mail.ru",
                        Name = "test",
                    },
                    Id = 1,
                    CountNegativeReviews = 0,
                    CountPositiveReviews = 0,
                    Rating = 5,
                    SendTime = new DateTime(2025, 5, 10),
                    TestId = 1,
                    Text = "some text here",
                    UpdateTime = new DateTime(2025, 5, 10)
                }

            };
            var expectedResult = new BasePaginationResponse<FeedbackWithReviewsResponse>
            {
                Items = expectedFeedbacks,
                MaxCount = 1,
                Page = page,
                PageCount = pageSize
            };

            externalServiceMock.Setup(x => x
                .TestIsExists(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbacksByCriteria(
                    It.IsAny<FeedbacksByTestIdWithOwnerAndReviewsSpecification>(), 
                    It.IsAny<int>(), 
                    It.IsAny<int>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedbacksFromDb);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbacks(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedbacksFromDb);

            // Act
            var feedbacksPagination = await handler.Handle(command, CancellationToken.None);

            // Assert
            feedbacksPagination.Items.Should().BeEquivalentTo(expectedFeedbacks);
            feedbacksPagination.Page.Should().Be(expectedResult.Page);
            feedbacksPagination.PageCount.Should().Be(expectedResult.PageCount);
            feedbacksPagination.MaxCount.Should().Be(expectedResult.MaxCount);

            unitOfWorkMock.Verify(x => x
                .BeginTransactionAsync(
                    It.IsAny<IsolationLevel>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            unitOfWorkMock.Verify(x => x
                .CommitTransactionAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
