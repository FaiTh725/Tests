using Application.Shared.Exceptions;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TestRating.Application.Commands.FeedbackReviewEntity.SendFeedbackReview;
using TestRating.Domain.Entities;

namespace TestRating.Integration.Tests.Application.Commands.Reviews
{
    public class SendFeedbackReviewCommandHandlerTests : BaseIntegrationTest
    {
        public SendFeedbackReviewCommandHandlerTests(
            CustomWebFactory factory) : base(factory)
        {}

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendFeedbackReviewCommand
            {
                FeedbackId = 1,
                IsPositive = true,
                ProfileId = 1
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var command = new SendFeedbackReviewCommand
            {
                FeedbackId = 1,
                IsPositive = true,
                ProfileId = profileFromDb.Entity.Id
            };

            // Act
            var act = async () => await sender.Send(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_IfSendReviewFirstTime_ThenAddNewReview()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("some text", 1, 
                5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var command = new SendFeedbackReviewCommand
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                IsPositive = true,
                ProfileId = profileFromDb.Entity.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert
            var review = await context.Reviews
                .SingleOrDefaultAsync();

            review.Should().NotBeNull();
            review.OwnerId.Should().Be(command.ProfileId);
            review.IsPositive.Should().Be(command.IsPositive);
            review.ReviewedFeedbackId.Should().Be(command.FeedbackId);
        }

        [Fact]
        public async Task Handle_IfSendSameReviewAsExistedForFeedback_ThenDeleteReview()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("some text", 1,
                5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var reviewEntity = FeedbackReview.Initialize(true, 
                profileFromDb.Entity.Id, feedbackFromDb.Entity.Id).Value;

            var reviewFromDb = await context.Reviews
                .AddAsync(reviewEntity);
            await context.SaveChangesAsync();

            var command = new SendFeedbackReviewCommand
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                IsPositive = true,
                ProfileId = profileFromDb.Entity.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert
            var review = await context.Reviews
                .FirstOrDefaultAsync(x => x.Id == reviewFromDb.Entity.Id);

            review.Should().BeNull();
        }

        [Fact]
        public async Task Handle_IfSendDifReviewAsExistedForFeedback_ThenUpdateReview()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles
                .AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("some text", 1,
                5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var reviewEntity = FeedbackReview.Initialize(false,
                profileFromDb.Entity.Id, feedbackFromDb.Entity.Id).Value;

            var reviewFromDb = await context.Reviews
                .AddAsync(reviewEntity);
            await context.SaveChangesAsync();

            var command = new SendFeedbackReviewCommand
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                IsPositive = true,
                ProfileId = profileFromDb.Entity.Id
            };

            // Act
            await sender.Send(command, CancellationToken.None);

            // Assert
            var review = await context.Reviews
                .SingleOrDefaultAsync();

            review.Should().NotBeNull();
            review.IsPositive.Should().Be(command.IsPositive);
        }
    }
}
