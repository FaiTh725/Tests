using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestRating.Application.Commands.FeedbackReportEntity.SendReport;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Reports
{
    [Collection("Integration Tests")]
    public class SendReportCommandHandlerTests : 
        BaseIntegrationTest
    {
        public SendReportCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var command = new SendReportCommand
            {
                Message = "report message",
                ReportedFeedbackId = 1,
                ReviewerId = profileFromDb.Entity.Id
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;
            
            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new SendReportCommand
            {
                Message = "report message",
                ReportedFeedbackId = feedbackFromDb.Entity.Id,
                ReviewerId = 5
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenCurProfileReportedFeedback_ShouldThrowConflictException()
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

            var command = new SendReportCommand
            {
                Message = "report message",
                ReportedFeedbackId = feedbackFromDb.Entity.Id,
                ReviewerId = profileFromDb.Entity.Id
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Current profile has already sent a report to current feedback");
        }

        [Fact]
        public async Task Handle_WhenInvalidCommand_ShouldBadRequestException()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new SendReportCommand
            {
                Message = "",
                ReportedFeedbackId = feedbackFromDb.Entity.Id,
                ReviewerId = profileFromDb.Entity.Id
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Handle_WhenCorrectCommand_ShouldAddReport()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new SendReportCommand
            {
                Message = "report message",
                ReportedFeedbackId = feedbackFromDb.Entity.Id,
                ReviewerId = profileFromDb.Entity.Id
            };

            // Act
            var reportId = await sender.Send(command, CancellationToken.None);

            // Assert
            var report = await context.Reports
                .FirstOrDefaultAsync(x => x.Id == reportId);

            report.Should().NotBeNull();
            report.ReportMessage.Should().Be(command.Message);
            report.ReportedFeedbackId.Should().Be(command.ReportedFeedbackId);
            report.ReviewerId.Should().Be(command.ReviewerId);
        }
    }
}
