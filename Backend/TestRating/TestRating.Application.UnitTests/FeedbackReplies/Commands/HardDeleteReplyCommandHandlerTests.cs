using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.ReplyEntity.DeleteReply;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReplies.Commands
{
    public class HardDeleteReplyCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackReplyRepository> replyReposityMock;

        private readonly DeleteReplyHandler handler;

        public HardDeleteReplyCommandHandlerTests()
        {
            unitOfWorkMock = new();
            replyReposityMock = new();

            handler = new DeleteReplyHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ReplyRepository)
                .Returns(replyReposityMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackReplyDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteReplyCommand
            {
                ReplyId = 1,
                ProfileId = 1,
                ProfileRole = "User"
            };

            replyReposityMock.Setup(x => x
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
        public async Task Handle_WhenFeedbackReplyExists_ShouldHardDeleteFeedback()
        {
            // Arrange
            var command = new DeleteReplyCommand
            {
                ReplyId = 1,
                ProfileId = 1,
                ProfileRole = "User"
            };
            var existedFeedback = FeedbackReply.Initialize("some text here", 1, 1).Value;

            replyReposityMock.Setup(x => x
                .GetReply(
                    command.ReplyId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            replyReposityMock.Verify(x => x
                .HardDeleteReply(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
