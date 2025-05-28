using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestRating.Application.Commands.FeedbackEntity.BanFeedback;
using TestRating.Application.Commands.FeedbackEntity.DeleteFeedback;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Feedbacks
{
    [Collection("Integration Tests")]
    public class BanFeedbackCommandHandlerTests : 
        BaseIntegrationTest
    {
        public BanFeedbackCommandHandlerTests(
            CustomWebFactory factory) : base(factory)
        {}

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new BanFeedbackCommand
            {
                Id = 1
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenFeedbackExist_ShouldBanFeedback()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;
            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new BanFeedbackCommand
            {
                Id = feedbackFromDb.Entity.Id,
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert

            var banedFeedback = await context.Feedbacks
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == feedbackFromDb.Entity.Id);

            banedFeedback.Should().NotBeNull();
            banedFeedback.IsDeleted.Should().BeTrue();
        }
    }
}
