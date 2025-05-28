using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TestRating.Application.Contacts.Feedback;
using TestRating.Application.Contacts.Pagination;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestId;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Queries.Feedbacks
{
    [Collection("Integration Tests")]
    public class GetFeedbacksByTestIdQueryHandlerTests : 
        BaseIntegrationTest
    {
        public GetFeedbacksByTestIdQueryHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handler_WhenTestDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetFeedbacksByTestIdQuery
            {
                TestId = 1,
                Page = 1,
                PageSize = 12
            };

            factory.TestExternalServiceMock.Setup(x => x
                .TestIsExists(
                    query.TestId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var act = async () => await sender.Send(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Test doesnt exist");
        }

        [Fact]
        public async Task Handler_WhenTestExists_ShouldReturnsFeedbacks()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackForTestEntity = Feedback.Initialize("feedback text", 1, 
                5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks
                .AddAsync(feedbackForTestEntity);
            await context.SaveChangesAsync();

            var feedbackReview1 = FeedbackReview.Initialize(true, 
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;
            var feedbackReview2 = FeedbackReview.Initialize(false,
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            await context.Reviews.AddRangeAsync([feedbackReview1, feedbackReview2]);
            await context.SaveChangesAsync();

            var expectedResult = new BasePaginationResponse<FeedbackWithReviewsResponse>
            {
                MaxCount = 1,
                Page = 1,
                PageCount = 12,
                Items = [
                    new FeedbackWithReviewsResponse{
                        Profile = new BaseProfileResponse{
                            Email = "test@mail.com",
                            Id = profileFromDb.Entity.Id,
                            Name = "test"
                        },
                        Id = feedbackFromDb.Entity.Id,
                        CountNegativeReviews = 1,
                        CountPositiveReviews = 1,
                        Rating = 5,
                        TestId = 1,
                        Text = "feedback text",
                    }
                    ]
            };

            var query = new GetFeedbacksByTestIdQuery
            {
                TestId = 1,
                Page = 1,
                PageSize = 12
            };

            factory.TestExternalServiceMock.Setup(x => x
                .TestIsExists(
                    query.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await sender.Send(query, CancellationToken.None);

            // Assert
            var expectedItems = expectedResult.Items.ToList();
            var actualItems = result.Items.ToList();

            result.Page.Should().Be(expectedResult.Page);
            result.PageCount.Should().Be(expectedResult.PageCount);
            result.MaxCount.Should().Be(expectedResult.MaxCount);
            actualItems[0].Id.Should().Be(expectedItems[0].Id);
            actualItems[0].Rating.Should().Be(expectedItems[0].Rating);
            actualItems[0].Text.Should().Be(expectedItems[0].Text);
            actualItems[0].TestId.Should().Be(expectedItems[0].TestId);
            actualItems[0].CountPositiveReviews.Should().Be(expectedItems[0].CountPositiveReviews);
            actualItems[0].CountNegativeReviews.Should().Be(expectedItems[0].CountNegativeReviews);
            actualItems[0].Profile.Should().BeEquivalentTo(expectedItems[0].Profile);
        }
    }
}
