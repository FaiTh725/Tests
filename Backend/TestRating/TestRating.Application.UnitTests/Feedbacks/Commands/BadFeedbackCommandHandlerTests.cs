using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.FeedbackEntity.BanFeedback;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Feedbacks.Commands
{
    public class BadFeedbackCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;

        private readonly BanFeedbackHandler handler;

        public BadFeedbackCommandHandlerTests()
        {
            unitOfWorkMock = new();
            feedbackRepositoryMock = new();

            handler = new BanFeedbackHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new BanFeedbackCommand
            {
                Id = 1
            };

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.Id, 
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
        public async Task Handle_WhenFeedbackExists_ShouldSoftDeleteFeedback()
        {
            // Arrange
            var command = new BanFeedbackCommand
            {
                Id = 1
            };
            var feedbackFromDb = Feedback.Initialize("good", 1, 5, 1).Value;
            var expectedDeletedTime = DateTime.UtcNow;

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedbackFromDb);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
            feedbackRepositoryMock.Verify(x => x
                .SoftDeleteFeedback(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
