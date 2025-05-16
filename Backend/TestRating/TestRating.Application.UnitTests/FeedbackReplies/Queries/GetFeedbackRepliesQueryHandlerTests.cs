using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using System.Data;
using TestRating.Application.Contacts.FeedbackReply;
using TestRating.Application.Contacts.Pagination;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReplyEntity.GetFeedbackReplies;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReplies.Queries
{
    public class GetFeedbackRepliesQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;
        private readonly Mock<IFeedbackReplyRepository> replyRepositoryMock;
    
        private readonly GetFeedbackRepliesHandler handler;

        public GetFeedbackRepliesQueryHandlerTests()
        {
            unitOfWorkMock = new();
            replyRepositoryMock = new();
            feedbackRepositoryMock = new();

            handler = new GetFeedbackRepliesHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ReplyRepository)
                .Returns(replyRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetFeedbackRepliesQuery 
            { 
                FeedbackId = 1,
                Page = 1,
                PageSize = 10
            };

            unitOfWorkMock.Setup(x => x.FeedbackRepository
                .GetFeedbackById(
                    It.IsAny<int>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Feedback);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenFeedbackExist_ShouldFeedbackReplies()
        {
            // Arrange
            var expectedReplies = new BasePaginationResponse<FeedbackReplyWithOwner>
            {
                Items = [
                    new FeedbackReplyWithOwner
                    {
                        Id = 1,
                        FeedbackId = 1,
                        Text = "some text",
                        SendTime = new DateTime(2025, 5, 5),
                        UpdateTime = new DateTime(2025, 5, 5),
                        Owner = new BaseProfileResponse
                        {
                            Id = 1,
                            Email = "zelenukho725@gmail.com",
                            Name = "FaiTh"
                        }
                    }],
                MaxCount = 1,
                Page = 1,
                PageCount = 10
            };
            var existedFeedback = Feedback.Initialize("some text", 1, 1, 1).Value;
            var query = new GetFeedbackRepliesQuery
            {
                FeedbackId = 1,
                Page = 1,
                PageSize = 10
            };
            var existedProfile = Profile.Initialize("zelenukho725@gmail.com", "FaiTh").Value;
            // set id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedProfile, [1]);

            var existedReply = FeedbackReply.Initialize("some text", 1, 1).Value;
            // set id
            type = typeof(FeedbackReply);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReply, [1]);
            //set owner
            property = type.GetProperty("Owner");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReply, [existedProfile]);
            // set update time
            property = type.GetProperty("UpdateTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReply, [new DateTime(2025, 5, 5)]);
            // set update time
            property = type.GetProperty("SendTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReply, [new DateTime(2025, 5, 5)]);

            var replies = new List<FeedbackReply>()
            {
                existedReply
            };

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    query.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            replyRepositoryMock.Setup(x => x
                .GetRepliesByCriteria(
                    It.IsAny<RepliesByFeedbackIdWithOwnerSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(replies);

            replyRepositoryMock.Setup(x => x
                .GetRepliesByCriteria(
                    It.IsAny<RepliesByFeedbackIdWithOwnerSpecification>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(replies);

            // Act
            var repliesResult = await handler.Handle(query, CancellationToken.None);

            // Assert
            repliesResult.Should().BeEquivalentTo(expectedReplies);

            unitOfWorkMock.Verify(x => x.BeginTransactionAsync(
                It.IsAny<IsolationLevel>(), 
                It.IsAny<CancellationToken>()), 
                Times.Once);

            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWorkMock.Verify(x => x.RollBackTransactionAsync(
                It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
