using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.FeedbackReportEntity.ReviewReport;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReports.Commands
{
    public class ReviewReportCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackReportRepository> reportRepositoryMock;

        private readonly ReviewReportHandler handler;

        public ReviewReportCommandHandlerTests()
        {
            unitOfWorkMock = new();
            reportRepositoryMock = new();

            handler = new ReviewReportHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ReportRepository)
                .Returns(reportRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackReportDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new ReviewReportCommand 
            { 
                IsApproved = true,
                ReportId = 1
            };

            reportRepositoryMock.Setup(x => x
                .GetFeedbackReport(
                    command.ReportId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReport);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback report doesnt exist");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Handle_WhenFeedbackReportExist_ShouldReviewReport(bool isApproved)
        {
            // Arrange
            var command = new ReviewReportCommand
            {
                IsApproved = isApproved,
                ReportId = 1
            };
            var existedReport = FeedbackReport.Initialize("some text", 1, 1).Value;

            reportRepositoryMock.Setup(x => x
                .GetFeedbackReport(
                    command.ReportId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReport);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            existedReport.IsApproval.Should().Be(command.IsApproved);

            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            reportRepositoryMock.Verify(x => x
                .UpdateFeedbackReport(
                    It.IsAny<long>(), 
                    It.IsAny<FeedbackReport>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
