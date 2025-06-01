using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestRating.Application.Commands.FeedbackEntity.ChangeFeedback;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Feedbacks
{
    [Collection("Integration Tests")]
    public class ChangeFeedbackCommandHandlerTests : BaseIntegrationTest
    {
        public ChangeFeedbackCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new ChangeFeedbackCommand
            {
                FeedbackId = 1,
                ProfileId = 1,
                Rating = 5,
                Text = "",
                ProfileRole = "Role"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Theory]
        [InlineData(-1, "some text")]
        [InlineData(20, "some text")]
        [InlineData(4, "")]
        public async Task Handle_WhenCommandIsIncorrect_ShouldThrowBadRequestException(int rate, string text)
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;
            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new ChangeFeedbackCommand
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                ProfileId = profileFromDb.Entity.Id,
                Rating = rate,
                Text = text,
                ProfileRole = "User"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Handle_WhenCommandIsCorrect_ShouldUpdateFeedback()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;
            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new ChangeFeedbackCommand
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                ProfileId = profileFromDb.Entity.Id,
                Rating = 2,
                Text = "new text",
                ProfileRole = "Admin"
            };

            // Act
            var updatedFeedbackId = await sender.Send(command, CancellationToken.None);

            // Assert
            var updatedFeedback = await context.Feedbacks
                .FirstOrDefaultAsync(x => x.Id == updatedFeedbackId);

            updatedFeedback.Should().NotBeNull();
            updatedFeedback.Text.Should().Be(command.Text);
            updatedFeedback.Rating.Should().Be(command.Rating);
        }
    }
}
