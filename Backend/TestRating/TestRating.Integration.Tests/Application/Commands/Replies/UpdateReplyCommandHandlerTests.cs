using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestRating.Application.Commands.ReplyEntity.UpdateReply;
using TestRating.Dal;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Replies
{
    [Collection("Integration Tests")]
    public class UpdateReplyCommandHandlerTests : 
        BaseIntegrationTest
    {
        public UpdateReplyCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenReplyDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var command = new UpdateReplyCommand
            {
                ProfileId = profileEntity.Id,
                ProfileRole = "User",
                Text = "new text",
                ReplyId = 1
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Reply doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenCommandIsIncorrect_ShouldThrowBadRequestException()
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
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var replyFromDb = await context.Replies.AddAsync(replyEntity);
            await context.SaveChangesAsync();

            var command = new UpdateReplyCommand
            {
                ProfileId = profileEntity.Id,
                ProfileRole = "User",
                Text = "",
                ReplyId = replyFromDb.Entity.Id
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Handle_WhenCommandIsCorrect_ShouldUpdateReply()
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
                feedbackFromDb.Entity.Id, profileFromDb.Entity.Id).Value;

            var replyFromDb = await context.Replies.AddAsync(replyEntity);
            await context.SaveChangesAsync();

            var command = new UpdateReplyCommand
            {
                ProfileId = profileEntity.Id,
                ProfileRole = "User",
                Text = "new text",
                ReplyId = replyFromDb.Entity.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert

            using var scope = serviceProvider.CreateScope();
            using var newContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var updatedReply = await newContext.Replies
                .FirstOrDefaultAsync(x => x.Id == command.ReplyId);

            updatedReply.Should().NotBeNull();
            updatedReply.Text.Should().Be(command.Text);
        }
    }
}
