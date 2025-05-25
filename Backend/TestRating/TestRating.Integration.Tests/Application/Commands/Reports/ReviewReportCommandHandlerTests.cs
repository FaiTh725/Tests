using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestRating.Application.Commands.FeedbackReportEntity.ReviewReport;
using TestRating.Dal;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Reports
{
    public class ReviewReportCommandHandlerTests : 
        BaseIntegrationTest
    {
        public ReviewReportCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenReportDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new ReviewReportCommand
            {
                IsApproved = true,
                ReportId = 1
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback report doesnt exist");
        }

        [Fact]
        public async Task Handle_IfApprovalReport_ThenBadFeedback()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var reportEntity = FeedbackReport.Initialize("report message",
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var reportFromDb = await context.Reports
                .AddAsync(reportEntity);
            await context.SaveChangesAsync();

            var command = new ReviewReportCommand
            {
                IsApproved = true,
                ReportId = reportFromDb.Entity.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert

            using var scope = serviceProvider.CreateScope();
            using var newContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var reportedFeedback = await newContext.Feedbacks
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == feedbackFromDb.Entity.Id);

            reportedFeedback!.IsDeleted.Should().BeTrue();

            var reviewedReport = await newContext.Reports
                .FirstOrDefaultAsync(x => x.Id == reportFromDb.Entity.Id);

            reviewedReport!.IsApproval.Should().Be(command.IsApproved);
        }

        [Fact]
        public async Task Handle_IfCancelReport_ThenNothingToDo()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var reportEntity = FeedbackReport.Initialize("report message",
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var reportFromDb = await context.Reports
                .AddAsync(reportEntity);
            await context.SaveChangesAsync();

            var command = new ReviewReportCommand
            {
                IsApproved = false,
                ReportId = reportFromDb.Entity.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert

            using var scope = serviceProvider.CreateScope();
            using var newContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var reviewedReport = await newContext.Reports
                .FirstOrDefaultAsync(x => x.Id == reportFromDb.Entity.Id);

            reviewedReport!.IsApproval.Should().Be(command.IsApproved);
        }
    }
}
