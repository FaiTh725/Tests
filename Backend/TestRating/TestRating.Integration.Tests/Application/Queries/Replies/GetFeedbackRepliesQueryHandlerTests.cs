using Application.Shared.Exceptions;
using FluentAssertions;
using TestRating.Application.Contacts.FeedbackReply;
using TestRating.Application.Contacts.Pagination;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReplyEntity.GetFeedbackReplies;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Queries.Replies
{
    [Collection("Integration Tests")]
    public class GetFeedbackRepliesQueryHandlerTests : 
        BaseIntegrationTest
    {
        public GetFeedbackRepliesQueryHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetFeedbackRepliesQuery
            {
                Page = 1,
                PageSize = 1,
                FeedbackId = 1
            };

            // Act
            var act = async () => await sender.Send(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenFeedbackExists_ShouldReturnsFeedbackReplies()
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

            var replyEntity = FeedbackReply.Initialize("reply text", 
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var replyFromDb = await context.Replies
                .AddAsync(replyEntity);
            await context.SaveChangesAsync();

            var expectedResult = new BasePaginationResponse<FeedbackReplyWithOwner>
            {
                MaxCount = 1,
                Page = 1,
                PageCount = 12,
                Items = [
                    new FeedbackReplyWithOwner 
                    { 
                        Id = replyFromDb.Entity.Id,
                        FeedbackId = replyFromDb.Entity.FeedbackId,
                        Text = replyFromDb.Entity.Text,
                        Owner = new BaseProfileResponse
                        {
                            Email = "test@mail.com",
                            Name = "test",
                            Id = profileFromDb.Entity.Id
                        }
                    }]
            };

            var query = new GetFeedbackRepliesQuery
            {
                Page = 1,
                PageSize = 12,
                FeedbackId = 1
            };

            // Act
            var result = await sender.Send(query, CancellationToken.None);

            // Assert
            var expectedItems = expectedResult.Items.ToList();
            var actualItems = result.Items.ToList();

            result.Page.Should().Be(expectedResult.Page);
            result.Page.Should().Be(expectedResult.Page);
            result.MaxCount.Should().Be(expectedResult.MaxCount);
            actualItems[0].Id.Should().Be(expectedItems[0].Id);
            actualItems[0].Text.Should().Be(expectedItems[0].Text);
            actualItems[0].FeedbackId.Should().Be(expectedItems[0].FeedbackId);
            actualItems[0].Owner.Should().BeEquivalentTo(expectedItems[0].Owner);
        }
    }
}
