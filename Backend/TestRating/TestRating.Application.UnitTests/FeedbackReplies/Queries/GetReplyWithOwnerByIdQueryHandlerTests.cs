using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Contacts.FeedbackReply;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReplyEntity.GetReplyWithOwnerById;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReplies.Queries
{
    public class GetReplyWithOwnerByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackReplyRepository> replyRepositoryMock;

        private readonly GetReplyWithOwnerByIdHandler handler;

        public GetReplyWithOwnerByIdQueryHandlerTests()
        {
            unitOfWorkMock = new();
            replyRepositoryMock = new();

            handler = new GetReplyWithOwnerByIdHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ReplyRepository)
                .Returns(replyRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenReplyDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetReplyWithOwnerByIdQuery
            {
                Id = 1
            };

            replyRepositoryMock.Setup(x => x
                .GetReplyByCriteria(
                    It.IsAny<ReplyByIdWithOwnerSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReply);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Feedback reply doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenReplyExist_ShouldReturnReply()
        {
            // Arrange
            var expectedReply = new FeedbackReplyWithOwner
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
            };
            var query = new GetReplyWithOwnerByIdQuery
            {
                Id = 1
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

            replyRepositoryMock.Setup(x => x
                .GetReplyByCriteria(
                    It.IsAny<ReplyByIdWithOwnerSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReply);

            // Act
            var replyWithOwner = await handler.Handle(query, CancellationToken.None);

            // Assert
            replyWithOwner.Should().BeEquivalentTo(expectedReply);
        }
    }
}
