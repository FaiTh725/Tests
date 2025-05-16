using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.FeedbackReportEntity.SendReport;
using TestRating.Application.Queries.FeedbackReportEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReports.Commands
{
    public class SendReportCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<IFeedbackReportRepository> reportRepositoryMock;

        private readonly SendReportHandler handler;

        public SendReportCommandHandlerTests()
        {
            unitOfWorkMock = new();
            feedbackRepositoryMock = new();
            profileRepositoryMock = new();
            reportRepositoryMock = new();

            handler = new SendReportHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ReportRepository)
                .Returns(reportRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendReportCommand()
            {
                Message = "some text",
                ReportedFeedbackId = 1,
                ReviewerId = 1
            };

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.ReportedFeedbackId, 
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
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendReportCommand()
            {
                Message = "some text",
                ReportedFeedbackId = 1,
                ReviewerId = 1
            };
            var existedFeedback = Feedback.Initialize("some text", 1, 2, 45).Value;

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.ReportedFeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.ReviewerId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileAlreadySentReportOnThisFeedback_ShouldThrowConflictException()
        {
            // Arrange
            var command = new SendReportCommand()
            {
                Message = "some text",
                ReportedFeedbackId = 1,
                ReviewerId = 1
            };
            var existedFeedback = Feedback.Initialize("some text", 1, 2, 45).Value;
            var existedProfile = Profile.Initialize("zelenukho725@gmail.com", "FaITh").Value;
            var existedReport = FeedbackReport.Initialize("report message", 1, 1).Value;

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.ReportedFeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.ReviewerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            reportRepositoryMock.Setup(x => x
                .GetFeedbackReportByCriteria(
                    It.IsAny<ReportProfileToFeedbackSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReport);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Current profile has already sent a report to current feedback");
        }

        [Fact]
        public async Task Handle_WhenProfileSentReportFirstly_ShouldAddReport()
        {
            // Arrange
            var command = new SendReportCommand()
            {
                Message = "some text",
                ReportedFeedbackId = 1,
                ReviewerId = 1
            };
            var existedFeedback = Feedback.Initialize("some text", 1, 2, 45).Value;
            var existedProfile = Profile.Initialize("zelenukho725@gmail.com", "FaITh").Value;
            var addedReport = FeedbackReport.Initialize("report message", 1, 1).Value;
            // set report id instead of db
            var type = typeof(FeedbackReport);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(addedReport, [1]);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackById(
                    command.ReportedFeedbackId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    command.ReviewerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            reportRepositoryMock.Setup(x => x
                .GetFeedbackReportByCriteria(
                    It.IsAny<ReportProfileToFeedbackSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReport);

            reportRepositoryMock.Setup(x => x
                .AddReport(
                    It.IsAny<FeedbackReport>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(addedReport);

            // Act
            var reportId = await handler.Handle(command, CancellationToken.None);

            // Assert
            reportId.Should().Be(addedReport.Id);

            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            reportRepositoryMock.Verify(x => x
                .AddReport(
                    It.IsAny<FeedbackReport>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
