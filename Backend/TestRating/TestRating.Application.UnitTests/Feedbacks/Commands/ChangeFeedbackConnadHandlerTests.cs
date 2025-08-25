using Application.Shared.Exceptions;
using FluentAssertions;
using MassTransit;
using Moq;
using TestRating.Application.Commands.FeedbackEntity.ChangeFeedback;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.File;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Feedbacks.Commands
{
    public class ChangeFeedbackConnadHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<IPublishEndpoint> publishEndpointMock;

        private readonly ChangeFeedbackHandler handler;

        public ChangeFeedbackConnadHandlerTests()
        {
            unitOfWorkMock = new();
            feedbackRepositoryMock = new();
            blobServiceMock = new();
            publishEndpointMock = new();

            handler = new ChangeFeedbackHandler(
                unitOfWorkMock.Object, 
                blobServiceMock.Object, 
                publishEndpointMock.Object);

            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new ChangeFeedbackCommand
            {
                FeedbackId = 1
            };

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
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
        public async Task Handle_WhenFeedbackExist_ShouldUpdateExistedFeedback()
        {
            // Arrange
            var command = new ChangeFeedbackCommand
            {
                FeedbackId = 1,
                NewImages = new(),
                ProfileId = 1,
                ProfileRole = "User",
                Rating = 4,
                Text = "Quick delivery"
            };
            var existedFeedback = Feedback.Initialize("old text", 1, 5, 1).Value;
            // set feedbackId
            var type = typeof(Feedback);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [1]);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.FeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            // Act
            var feedbackId = await handler.Handle(command, CancellationToken.None);

            // Assert
            feedbackId.Should().Be(existedFeedback.Id);
            existedFeedback.Text.Should().Be(command.Text);
            existedFeedback.Rating.Should().Be(command.Rating);

            publishEndpointMock.Verify(x => x
                .Publish(
                    It.IsAny<ClearBlobFromStorage>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
            feedbackRepositoryMock.Verify(x => x
                .UpdateFeedback(
                    It.IsAny<long>(),
                    It.IsAny<Feedback>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            blobServiceMock.Verify(x => x
                .UploadBlobs(
                    It.IsAny<string>(),
                    It.IsAny<List<FileModel>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
