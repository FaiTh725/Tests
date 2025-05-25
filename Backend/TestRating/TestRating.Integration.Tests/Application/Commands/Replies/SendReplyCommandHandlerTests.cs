using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestRating.Application.Commands.ReplyEntity.SendReply;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Replies
{
    public class SendReplyCommandHandlerTests : 
        BaseIntegrationTest
    {
        public SendReplyCommandHandlerTests(CustomWebFactory factory) : 
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

            var command = new SendReplyCommand
            {
                FeedbackId = 1,
                OwnerId = profileFromDb.Entity.Id,
                Text = "some text here"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileAlreadySentReplyCurFeedback_ShouldThrowConflictException()
        {
            // Arrange 
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("feedback text", 1, 5, 
                profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var replyEntity = FeedbackReply.Initialize("reply text", 
                feedbackFromDb.Entity.Id, feedbackFromDb.Entity.Id).Value;

            await context.Replies.AddAsync(replyEntity);
            await context.SaveChangesAsync();

            var command = new SendReplyCommand
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                OwnerId = profileFromDb.Entity.Id,
                Text = "some text here"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Current profile has already sent a reply on this feedback");
        }

        [Fact]
        public async Task Handle_WhenInvalidCommand_ShouldThrowBadRequestException()
        {
            // Arrange 
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("feedback text", 1, 5,
                profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new SendReplyCommand
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                OwnerId = profileFromDb.Entity.Id,
                Text = ""
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Handle_WhenCommandIsCorrect_ShouldReturnReplyId()
        {
            // Arrange 
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("feedback text", 1, 5,
                profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new SendReplyCommand
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                OwnerId = profileFromDb.Entity.Id,
                Text = "reply text"
            };

            // Act
            var replyId =  await sender.Send(command, CancellationToken.None);

            // Assert
            var reply = await context.Replies
                .FirstOrDefaultAsync(x => x.Id == replyId);

            reply.Should().NotBeNull();
            reply.Text.Should().Be(command.Text);
            reply.FeedbackId.Should().Be(command.FeedbackId);
            reply.OwnerId.Should().Be(command.OwnerId);
        }
    }
}
