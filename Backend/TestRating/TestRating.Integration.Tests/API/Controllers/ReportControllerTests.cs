using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Cryptography.Xml;
using TestRating.API.Contracts.FeedbackReport;
using TestRating.Application.Commands.FeedbackReportEntity.ReviewReport;
using TestRating.Dal;
using TestRating.Domain.Entities;
using TestRating.Integration.Tests.Extensions;
using TestRating.Integration.Tests.JwtToken;
using Xunit.Abstractions;

namespace TestRating.Integration.Tests.API.Controllers
{
    public class ReportControllerTests : 
        BaseIntegrationTest
    {
        private readonly ITestOutputHelper testLogger;

        public ReportControllerTests(
            CustomWebFactory factory,
            ITestOutputHelper testLogger) : 
            base(factory)
        {
            this.testLogger = testLogger;
        }

        [Fact]
        public async Task SendReport_WhenFeedbackDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var request = new SendReportRequest
            {
                ReportedFeedbackId = 1,
                Message = "text"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Role = "Admin",
                Name = "test"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Report/SendReport", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendReport_WhenUserAlreadySentReport_ShouldReturns409Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("some text here", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks
                .AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var reportEntity = FeedbackReport.Initialize("report message", 
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;
            await context.Reports.AddAsync(reportEntity);
            await context.SaveChangesAsync();

            var request = new SendReportRequest
            {
                ReportedFeedbackId = feedbackFromDb.Entity.Id,
                Message = "text"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Role = "Admin",
                Name = "test"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Report/SendReport", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task SendReport_WhenIncorrectReport_ShouldReturns400Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("some text here", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks
                .AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var request = new SendReportRequest
            {
                ReportedFeedbackId = feedbackFromDb.Entity.Id,
                Message = ""
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Role = "Admin",
                Name = "test"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Report/SendReport", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendReport_WhenRequestIsCorrect_ShouldReturns200Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("some text here", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks
                .AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var request = new SendReportRequest
            {
                ReportedFeedbackId = feedbackFromDb.Entity.Id,
                Message = "report message"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Role = "Admin",
                Name = "test"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Report/SendReport", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var report = await context.Reports.SingleOrDefaultAsync();
            
            report.Should().NotBeNull();
            report.ReviewerId.Should().Be(profileFromDb.Entity.Id);
            report.ReportMessage.Should().Be(request.Message);
            report.ReportedFeedbackId.Should().Be(request.ReportedFeedbackId);
        }

        [Fact]
        public async Task ReviewReport_WhenReportDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var request = new ReviewReportCommand
            {
                IsApproved = true,
                ReportId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Report/ReviewReport", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ReviewReport_WhenReportApproved_ShouldReturns204StatusAndBadFeedback()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("some text here", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks
                .AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var reportEntity = FeedbackReport.Initialize("some text here", 
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var reportFromDb = await context.Reports
                .AddAsync(reportEntity);
            await context.SaveChangesAsync();

            var request = new ReviewReportCommand
            {
                IsApproved = true,
                ReportId = reportFromDb.Entity.Id
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Report/ReviewReport", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = serviceProvider.CreateScope();
            using var newContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var updatedReport = await newContext.Reports
                .FirstOrDefaultAsync(x => x.Id == reportFromDb.Entity.Id);
            
            updatedReport.Should().NotBeNull();
            updatedReport.IsApproval.Should().Be(request.IsApproved);

            var bannedFeedback = await newContext.Feedbacks
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == feedbackFromDb.Entity.Id);

            bannedFeedback.Should().NotBeNull();
            bannedFeedback.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task ReviewReport_WhenReportNotApproved_ShouldReturns204StatusAndBadFeedback()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("some text here", 1, 5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks
                .AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var reportEntity = FeedbackReport.Initialize("some text here",
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var reportFromDb = await context.Reports
                .AddAsync(reportEntity);
            await context.SaveChangesAsync();

            var request = new ReviewReportCommand
            {
                IsApproved = false,
                ReportId = reportFromDb.Entity.Id
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Report/ReviewReport", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = serviceProvider.CreateScope();
            using var newContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var updatedReport = await newContext.Reports
                .FirstOrDefaultAsync(x => x.Id == reportFromDb.Entity.Id);

            updatedReport.Should().NotBeNull();
            updatedReport.IsApproval.Should().Be(request.IsApproved);

            var bannedFeedback = await newContext.Feedbacks
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == feedbackFromDb.Entity.Id);

            bannedFeedback.Should().NotBeNull();
            bannedFeedback.IsDeleted.Should().BeFalse();
        }
    }
}
