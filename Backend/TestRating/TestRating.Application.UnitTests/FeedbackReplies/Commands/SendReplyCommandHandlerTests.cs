using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using System.Reflection;
using TestRating.Application.Commands.ReplyEntity.SendReply;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReplies.Commands
{
    public class SendReplyCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;
        private readonly Mock<IFeedbackReplyRepository> replyRepositoryMock;

        private readonly SendReplyHandler handler;

        public SendReplyCommandHandlerTests()
        {
            unitOfWorkMock = new();
            feedbackRepositoryMock = new();
            replyRepositoryMock = new();

            handler = new SendReplyHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ReplyRepository)
                .Returns(replyRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendReplyCommand
            {
                FeedbackId = 1,
                OwnerId = 1,
                Text = "some text here"
            };

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Feedback);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileAlreadySentReplyOnCurFeedback_ShouldThrowConflictException()
        {
            // Arrange
            var command = new SendReplyCommand
            {
                FeedbackId = 1,
                OwnerId = 1,
                Text = "some text here"
            };
            var feedback = Feedback.Initialize("some text here", 1, 5, 1).Value;
            var existedReply = FeedbackReply.Initialize("some text here", 1, 1).Value;

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedback);

            replyRepositoryMock.Setup(x => x
                .GetReplyByCriteria(
                    It.IsAny<ReplyByOwnerAndFeedbackIdSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReply);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Current profile has already sent a reply on this feedback");
        }

        [Fact]
        public async Task Handle_WhenFeedbackExistAndProfileFirstlySentReply_ShouldAddReply()
        {
            // Arrange
            var command = new SendReplyCommand
            {
                FeedbackId = 1,
                OwnerId = 1,
                Text = "some text here"
            };
            var feedback = Feedback.Initialize("some text here", 1, 5, 1).Value;
            var addedReply = FeedbackReply.Initialize(command.Text, command.FeedbackId, command.OwnerId).Value;
            // set id instead of db
            var type = typeof(FeedbackReply);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(addedReply, [1]);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedback);

            replyRepositoryMock.Setup(x => x
                .GetReplyByCriteria(
                    It.IsAny<ReplyByOwnerAndFeedbackIdSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReply);

            replyRepositoryMock.Setup(x => x
                .AddReply(
                    It.IsAny<FeedbackReply>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedReply);

            // Act
            var feedbackId = await handler.Handle(command, CancellationToken.None);

            // Assert
            feedbackId.Should().Be(addedReply.Id);

            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            replyRepositoryMock.Verify(x => x
                .AddReply(
                    It.IsAny<FeedbackReply>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
