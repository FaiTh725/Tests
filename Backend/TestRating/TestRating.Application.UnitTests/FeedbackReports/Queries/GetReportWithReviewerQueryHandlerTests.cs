using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Contacts.FeedbackReport;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReportEntity.GetReportWithReviewer;
using TestRating.Application.Queries.FeedbackReportEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.FeedbackReports.Queries
{
    public class GetReportWithReviewerQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IFeedbackReportRepository> reportRepositoryMock;

        private readonly GetReportWithReviewerHandler handler;

        public GetReportWithReviewerQueryHandlerTests()
        {
            unitOfWorkMock = new();
            reportRepositoryMock = new();
            
            handler = new GetReportWithReviewerHandler(unitOfWorkMock.Object);
            
            unitOfWorkMock.Setup(x => x.ReportRepository)
                .Returns(reportRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenReportDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetReportWithReviewerQuery 
            { 
                Id = 1 
            };
            
            reportRepositoryMock.Setup(x => x
                .GetFeedbackReportByCriteria(
                    It.IsAny<ReportWithReviewerSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as FeedbackReport);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);
            
            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Feedback report doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenReportExist_ShouldReturnReportWithReviewer()
        {
            // Arrange
            var expectedReport = new FeedbackReportWithReviewer
            {
                Id = 1,
                IsApproval = true,
                CreatedTime = new DateTime(2025, 5, 5),
                FeedbackId = 1,
                ReportMessage = "some text",
                Reviewer = new BaseProfileResponse
                {
                    Id = 1,
                    Email = "zelenukho725@gmail.com",
                    Name = "FaiTh"
                }
            };

            var query = new GetReportWithReviewerQuery
            {
                Id = 1
            };
            var existedProfile = Profile.Initialize("zelenukho725@gmail.com", "FaiTh").Value;
            // set id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedProfile, [1]);
            var existedReport = FeedbackReport.Initialize("some text", 1, 1).Value;
            // set id
            type = typeof(FeedbackReport);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReport, [1]);
            // set created time
            type = typeof(FeedbackReport);
            property = type.GetProperty("CreatedTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReport, [new DateTime(2025, 5, 5)]);
            // set reviewer
            type = typeof(FeedbackReport);
            property = type.GetProperty("Reviewer");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedReport, [existedProfile]);
            existedReport.ReviewReport(true);

            reportRepositoryMock.Setup(x => x
                .GetFeedbackReportByCriteria(
                    It.IsAny<ReportWithReviewerSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedReport);

            // Act
            var report = await handler.Handle(query, CancellationToken.None);

            // Assert
            report.Should().BeEquivalentTo(expectedReport);
        }
    }
}
