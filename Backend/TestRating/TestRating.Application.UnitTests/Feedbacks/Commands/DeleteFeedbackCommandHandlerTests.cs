using Application.Shared.Exceptions;
using FluentAssertions;
using MassTransit;
using Moq;
using TestRating.Application.Commands.FeedbackEntity.DeleteFeedback;
using TestRating.Application.Contacts.File;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Feedbacks.Commands
{
    public class DeleteFeedbackCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IPublishEndpoint> publishEndpointMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;

        private readonly DeleteFeedbackHandler handler;

        public DeleteFeedbackCommandHandlerTests()
        {
            unitOfWorkMock = new();
            publishEndpointMock = new();
            feedbackRepositoryMock = new();

            handler = new DeleteFeedbackHandler(
                unitOfWorkMock.Object, publishEndpointMock.Object);

            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteFeedbackCommand()
            { 
                FeedbackId = 1,
                ProfileId = 1,
                ProfileRole = "Admin"
            };

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackExcludeFiltersById(
                    command.FeedbackId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Feedback);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenFeedbackExist_ShouldHardDeleteFeedback()
        {
            // Arrange
            var command = new DeleteFeedbackCommand()
            {
                FeedbackId = 1,
                ProfileId = 1,
                ProfileRole = "Admin"
            };
            var existedFeedback = Feedback.Initialize("some text", 1, 5, 2).Value;

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackExcludeFiltersById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            feedbackRepositoryMock.Verify(x => x
                .HardDeleteFeedback(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
            publishEndpointMock.Verify(x => x
                .Publish(
                    It.IsAny<ClearBlobFromStorage>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
