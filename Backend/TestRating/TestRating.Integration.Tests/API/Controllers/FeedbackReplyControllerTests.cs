using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TestRating.API.Contracts.FeedbackReply;
using TestRating.Dal;
using TestRating.Domain.Entities;
using TestRating.Integration.Tests.Extensions;
using TestRating.Integration.Tests.JwtToken;

namespace TestRating.Integration.Tests.API.Controllers
{
    [Collection("Integration Tests")]
    public class FeedbackReplyControllerTests : 
        BaseIntegrationTest
    {
        public FeedbackReplyControllerTests(
            CustomWebFactory factory) : base(factory)
        {}

        [Fact]
        public async Task SendFeedbackReply_WhenFeedbackDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var request = new SendReplyRequest
            {
                FeedbackId = 1,
                Text = "some text"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/FeedbackReply/SendFeedbackReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendFeedbackReply_WhenProfileSentReply_ShouldReturns409Status()
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

            var feedbackReply = FeedbackReply.Initialize("reply text", 
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            await context.Replies.AddAsync(feedbackReply);
            await context.SaveChangesAsync();

            var request = new SendReplyRequest
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                Text = "some text"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/FeedbackReply/SendFeedbackReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task SendFeedbackReply_WhenRequestIsIncorrect_ShouldReturns400Status()
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

            var request = new SendReplyRequest
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                Text = ""
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/FeedbackReply/SendFeedbackReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendFeedbackReply_WhenRequestIsCorrect_ShouldReturns200StatusAndAddFeedbackReply()
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

            var request = new SendReplyRequest
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                Text = "reply message here"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/FeedbackReply/SendFeedbackReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var reply = await context.Replies.SingleOrDefaultAsync();

            reply.Should().NotBeNull();
            reply.Text.Should().Be(request.Text);
            reply.FeedbackId.Should().Be(request.FeedbackId);
            reply.OwnerId.Should().Be(profileFromDb.Entity.Id);
        }

        [Fact]
        public async Task DeleteFeedbackReply_WhenReplyDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var request = new DeleteReplyRequest
            {
                ReplyId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/FeedbackReply/DeleteFeedbackReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteFeedbackReply_WhenReplyExists_ShouldReturns204StatusAndDeleteReply()
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

            var replyEntity = FeedbackReply.Initialize("reply text", 
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var replyFromDb = await context.Replies
                .AddAsync(replyEntity);
            await context.SaveChangesAsync();

            var request = new DeleteReplyRequest
            {
                ReplyId = replyFromDb.Entity.Id
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/FeedbackReply/DeleteFeedbackReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedReply = await context.Replies
                .FirstOrDefaultAsync(x => x.Id == request.ReplyId);

            deletedReply.Should().BeNull();
        }

        [Fact]
        public async Task ChangeReply_WhenReplyDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var request = new ChangeReplyRequest
            {
                ReplyId = 1,
                Text = "new text"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/FeedbackReply/ChangeReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ChangeReply_WhenRequestIsIncorrect_ShouldReturns400Status()
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

            var replyEntity = FeedbackReply.Initialize("reply text",
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var replyFromDb = await context.Replies
                .AddAsync(replyEntity);
            await context.SaveChangesAsync();

            var request = new ChangeReplyRequest
            {
                ReplyId = replyFromDb.Entity.Id,
                Text = ""
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/FeedbackReply/ChangeReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ChangeReply_WhenRequestIsCorrect_ShouldReturns200Status()
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

            var replyEntity = FeedbackReply.Initialize("reply text",
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var replyFromDb = await context.Replies
                .AddAsync(replyEntity);
            await context.SaveChangesAsync();

            var request = new ChangeReplyRequest
            {
                ReplyId = replyFromDb.Entity.Id,
                Text = "new text"
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/FeedbackReply/ChangeReply", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var scope = serviceProvider.CreateScope();
            using var newContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var updatedReply = await newContext.Replies
                .FirstOrDefaultAsync(x => x.Id == request.ReplyId);
        
            updatedReply.Should().NotBeNull();
            updatedReply.Text.Should().Be(request.Text);
        }
    }
}
