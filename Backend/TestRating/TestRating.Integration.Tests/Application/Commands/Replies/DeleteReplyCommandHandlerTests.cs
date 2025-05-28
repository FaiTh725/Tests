using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestRating.Application.Commands.ReplyEntity.DeleteReply;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Replies
{
    [Collection("Integration Tests")]
    public class DeleteReplyCommandHandlerTests : 
        BaseIntegrationTest
    {
        public DeleteReplyCommandHandlerTests(CustomWebFactory factory) : 
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

            var command = new DeleteReplyCommand
            {
                ProfileId = profileFromDb.Entity.Id,
                ProfileRole = "User",
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
        public async Task Handle_WhenReplyExist_ShouldDeleteReply()
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

            var command = new DeleteReplyCommand
            {
                ProfileId = profileFromDb.Entity.Id,
                ProfileRole = "User",
                ReplyId = replyFromDb.Entity.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert
            var deletedReply = await context.Replies
                .FirstOrDefaultAsync(x => x.Id == command.ReplyId);
        
            deletedReply.Should().BeNull();
        }
    }
}
