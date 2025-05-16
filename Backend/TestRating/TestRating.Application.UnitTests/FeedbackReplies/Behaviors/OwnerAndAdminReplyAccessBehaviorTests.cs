using Application.Shared.Exceptions;
using FluentAssertions;
using MediatR;
using Moq;
using TestRating.Application.Behaviors;
using TestRating.Application.Commands.ReplyEntity.UpdateReply;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReplies.Behaviors
{
    public class OwnerAndAdminReplyAccessBehaviorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IFeedbackReplyRepository> replyRepositoryMock;
        
        private readonly OwnerAndAdminReplyAccessBehavior<UpdateReplyCommand, string> behavior;
        public OwnerAndAdminReplyAccessBehaviorTests()
        {
            unitOfWork = new();
            replyRepositoryMock = new();
            
            behavior = new OwnerAndAdminReplyAccessBehavior<UpdateReplyCommand, string>(unitOfWork.Object);
            
            unitOfWork.Setup(x => x.ReplyRepository)
                .Returns(replyRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenReplyDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new UpdateReplyCommand
            {
                ReplyId = 1,
                ProfileId = 2,
                ProfileRole = "User",
                Text = "some text"
            };

            replyRepositoryMock.Setup(x => x
                .GetReply(
                    command.ProfileId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReply);

            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            var act = async () => await behavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback Reply doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenUserIsNotOwner_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var command = new UpdateReplyCommand
            {
                ReplyId = 1,
                ProfileId = 1,
                ProfileRole = "User",
                Text = "some text"
            };
            var replyOwner = Profile.Initialize("test@mail.com", "FaiTh").Value;
            // set Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(replyOwner, [2]);
            var existedReply = FeedbackReply.Initialize("some text", 1, 2).Value;
            // set Owner
            type = typeof(FeedbackReply);
            property = type.GetProperty("Owner");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReply, [replyOwner]);

            replyRepositoryMock.Setup(x => x
                .GetReply(
                    command.ProfileId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReply);

            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            var act = async () => await behavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ForbiddenAccessException>()
                .WithMessage("Only the owner and an admin have access to the feedback");
        }

        [Fact]
        public async Task Handle_WhenUserIsAdmin_ShouldExecuteNext()
        {
            // Arrange
            var command = new UpdateReplyCommand
            {
                ReplyId = 1,
                ProfileId = 1,
                ProfileRole = "Admin",
                Text = "some text"
            };
            var expectedResponse = "Something";

            var replyOwner = Profile.Initialize("test@mail.com", "FaiTh").Value;
            // set Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(replyOwner, [2]);
            
            var existedReply = FeedbackReply.Initialize("some text", 1, 1).Value;
            // set Owner
            type = typeof(FeedbackReply);
            property = type.GetProperty("Owner");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReply, [replyOwner]);

            replyRepositoryMock.Setup(x => x
                .GetReply(
                    command.ProfileId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReply);

            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");

            // Act
            var nextResponse = await behavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            nextResponse.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task Handle_WhenUserIsOwner_ShouldExecuteNext()
        {
            // Arrange
            var command = new UpdateReplyCommand
            {
                ReplyId = 1,
                ProfileId = 1,
                ProfileRole = "User",
                Text = "some text"
            };
            var expectedResponse = "Something";

            var replyOwner = Profile.Initialize("test@mail.com", "FaiTh").Value;
            // set Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(replyOwner, [1]);

            var existedReply = FeedbackReply.Initialize("some text", 1, 1).Value;
            // set Owner
            type = typeof(FeedbackReply);
            property = type.GetProperty("Owner");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReply, [replyOwner]);

            replyRepositoryMock.Setup(x => x
                .GetReply(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReply);

            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");

            // Act
            var nextResponse = await behavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            nextResponse.Should().Be(expectedResponse);
        }
    }
}
