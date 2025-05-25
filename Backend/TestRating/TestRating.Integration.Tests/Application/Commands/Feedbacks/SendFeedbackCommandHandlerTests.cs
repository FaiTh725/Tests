using Application.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TestRating.Application.Commands.FeedbackEntity.SendFeedback;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Feedbacks
{
    public class SendFeedbackCommandHandlerTests : 
        BaseIntegrationTest
    {
        public SendFeedbackCommandHandlerTests(CustomWebFactory factory) : 
            base(factory)
        {
            factory.TestExternalServiceMock.Setup(x => x
                .TestIsExists(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendFeedbackCommand
            {
                ProfileId = 1,
                Rating = 5,
                TestId = 1,
                Text = "some text"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileAlreadySentForCurTest_ShouldThrowConflictException()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profileFromDb.Entity.Id).Value;
            await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new SendFeedbackCommand
            {
                ProfileId = profileFromDb.Entity.Id,
                Rating = 5,
                TestId = 1,
                Text = "some text"
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Feedback for this test has already sent, update old");
        }

        [Theory]
        [InlineData(-1, "some text")]
        [InlineData(20, "some text")]
        [InlineData(4, "")]
        public async Task Handle_WhenCommandIsIncorrect_ShouldBadRequestException(int rating, string text)
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var command = new SendFeedbackCommand
            {
                ProfileId = profileFromDb.Entity.Id,
                Rating = rating,
                TestId = 1,
                Text = text
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Handle_WhenCommandIsCorrect_ShouldReturnFeedbackId()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var command = new SendFeedbackCommand
            {
                ProfileId = profileFromDb.Entity.Id,
                Rating = 5,
                TestId = 1,
                Text = "some text"
            };

            // Act
            var feedbackId = await sender.Send(command, CancellationToken.None);

            // Assert
            var feedback = await context.Feedbacks
                .FirstOrDefaultAsync(x => x.Id == feedbackId);

            feedback.Should().NotBeNull();
            feedback.Rating.Should().Be(command.Rating);
            feedback.Text.Should().Be(command.Text);
            feedback.TestId.Should().Be(command.TestId);
            feedback.OwnerId.Should().Be(command.ProfileId);
        }
    }
}
