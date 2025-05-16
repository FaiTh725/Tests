using Application.Shared.Exceptions;
using FluentAssertions;
using MediatR;
using Moq;
using TestRating.Application.Behaviors;
using TestRating.Application.Commands.FeedbackEntity.DeleteFeedback;
using TestRating.Application.Commands.ReplyEntity.UpdateReply;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Feedbacks.Behaviors
{
    public class OwnerAndAdminFeedbackAccessBehaviorTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;
    
        private readonly OwnerAndAdminFeedbackAccessBehavior<DeleteFeedbackCommand, string> behavior;

        public OwnerAndAdminFeedbackAccessBehaviorTests()
        {
            unitOfWorkMock = new();
            feedbackRepositoryMock = new();

            behavior = new OwnerAndAdminFeedbackAccessBehavior<DeleteFeedbackCommand, string>(unitOfWorkMock.Object);
            
            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteFeedbackCommand
            {
                FeedbackId = 1,
                ProfileId = 2,
                ProfileRole = "User"
            };

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Feedback);

            var next = new Mock<RequestHandlerDelegate<string>>();

            // Act
            var act = async () => await behavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenUserIsNotOwner_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var command = new DeleteFeedbackCommand
            {
                FeedbackId = 1,
                ProfileId = 1,
                ProfileRole = "User"
            };

            var feedbackOwner = Profile.Initialize("test@mail.com", "FaiTh").Value;
            // set Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(feedbackOwner, [2]);
            
            var existedFeedback = Feedback.Initialize("some text", 1, 5, 2).Value;
            // set Owner
            type = typeof(Feedback);
            property = type.GetProperty("Owner");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [feedbackOwner]);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

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
            var command = new DeleteFeedbackCommand
            {
                FeedbackId = 1,
                ProfileId = 1,
                ProfileRole = "Admin"
            };
            var expectedResult = "Something";

            var feedbackOwner = Profile.Initialize("test@mail.com", "FaiTh").Value;
            // set Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(feedbackOwner, [2]);

            var existedFeedback = Feedback.Initialize("some text", 1, 5, 2).Value;
            // set Owner
            type = typeof(Feedback);
            property = type.GetProperty("Owner");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [feedbackOwner]);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");

            // Act
            var resultNext =  await behavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            resultNext.Should().Be(expectedResult);
        }

        [Fact]
        public async Task Handle_WhenUserIsOwner_ShouldExecuteNext()
        {
            // Arrange
            var command = new DeleteFeedbackCommand
            {
                FeedbackId = 1,
                ProfileId = 1,
                ProfileRole = "User"
            };
            var expectedResult = "Something";

            var feedbackOwner = Profile.Initialize("test@mail.com", "FaiTh").Value;
            // set Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(feedbackOwner, [1]);

            var existedFeedback = Feedback.Initialize("some text", 1, 5, 1).Value;
            // set Owner
            type = typeof(Feedback);
            property = type.GetProperty("Owner");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [feedbackOwner]);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            var next = new Mock<RequestHandlerDelegate<string>>();
            next.Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");

            // Act
            var resultNext = await behavior.Handle(command, next.Object, CancellationToken.None);

            // Assert
            resultNext.Should().Be(expectedResult);

        }
    }
}
