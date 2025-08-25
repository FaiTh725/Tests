using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.ReplyEntity.UpdateReply;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReplies.Commands
{
    public class UpdateFeedbackReplyCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackReplyRepository> replyRepositoryMock;

        private readonly UpdateReplyHandler handler;

        public UpdateFeedbackReplyCommandHandlerTests()
        {
            unitOfWorkMock = new();
            replyRepositoryMock = new();

            handler = new UpdateReplyHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ReplyRepository)
                .Returns(replyRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackReplyDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new UpdateReplyCommand
            {
                Text = "some text",
                ProfileId = 1,
                ProfileRole = "User",
                ReplyId = 1
            };

            replyRepositoryMock.Setup(x => x
                .GetReply(
                    command.ReplyId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReply);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Reply doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenFeedbackReplyExist_ShouldUpdateFeedbackReply()
        {
            // Arrange
            var command = new UpdateReplyCommand
            {
                Text = "some text",
                ProfileId = 1,
                ProfileRole = "User",
                ReplyId = 1
            };
            var existedReply = FeedbackReply.Initialize("Old text", 1, 1).Value;

            replyRepositoryMock.Setup(x => x
                .GetReply(
                    command.ReplyId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReply);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            existedReply.Text.Should().Be(command.Text);

            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            replyRepositoryMock.Verify(x => x
                .UpdateReply(
                    It.IsAny<long>(), 
                    It.IsAny<FeedbackReply>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
