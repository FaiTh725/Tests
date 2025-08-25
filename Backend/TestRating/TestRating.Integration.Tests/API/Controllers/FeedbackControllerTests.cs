using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TestRating.API.Contracts.Feedback;
using TestRating.Dal;
using TestRating.Domain.Entities;
using TestRating.Integration.Tests.Extensions;
using TestRating.Integration.Tests.JwtToken;
using Xunit.Abstractions;

namespace TestRating.Integration.Tests.API.Controllers
{
    [Collection("Integration Tests")]
    public class FeedbackControllerTests : 
        BaseIntegrationTest
    {
        private readonly ITestOutputHelper testLogger;

        public FeedbackControllerTests(CustomWebFactory factory, 
            ITestOutputHelper testLogger) : 
            base(factory)
        {
            this.testLogger = testLogger;
        }

        [Fact]
        public async Task SendFeedback_WhenUserIsUnauthorized_ShouldReturns401Status()
        {
            // Arrange
            var request = new CreateFeedbackRequest
            {
                Rating = 5,
                TestId = 1,
                Text = "text for test"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Feedback/SendFeedback", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task SendFeedback_WhenTestDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var request = new CreateFeedbackRequest
            {
                Rating = 5,
                TestId = 1,
                Text = "text for test"
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Rating.ToString()), "Rating" },
                { new StringContent(request.TestId.ToString()), "TestId" },
                { new StringContent(request.Text), "Text" }
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();

            factory.TestExternalServiceMock.Setup(x => x
                .TestIsExists(
                    request.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var httpResponse = await client.PostFormData("/api/Feedback/SendFeedback", content, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendFeedback_WhenProfileDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var request = new CreateFeedbackRequest
            {
                Rating = 5,
                TestId = 1,
                Text = "text for test"
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Rating.ToString()), "Rating" },
                { new StringContent(request.TestId.ToString()), "TestId" },
                { new StringContent(request.Text), "Text" }
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            factory.TestExternalServiceMock.Setup(x => x
                .TestIsExists(
                    request.TestId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var httpResponse = await client.PostFormData("/api/Feedback/SendFeedback", content, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendFeedback_WhenRequestContainsInvalidData_ShouldReturns400Status()
        {
            // Arrange
            var request = new CreateFeedbackRequest
            {
                Rating = 12,
                TestId = 1,
                Text = "text for test"
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Rating.ToString()), "Rating" },
                { new StringContent(request.TestId.ToString()), "TestId" },
                { new StringContent(request.Text), "Text" }
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();

            factory.TestExternalServiceMock.Setup(x => x
                .TestIsExists(
                    request.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var httpResponse = await client.PostFormData("/api/Feedback/SendFeedback", content, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendFeedback_WhenRequestIsCorrect_ShouldReturns200Status()
        {
            // Arrange
            var request = new CreateFeedbackRequest
            {
                Rating = 5,
                TestId = 1,
                Text = "text for test"
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Rating.ToString()), "Rating" },
                { new StringContent(request.TestId.ToString()), "TestId" },
                { new StringContent(request.Text), "Text" }
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            factory.TestExternalServiceMock.Setup(x => x
                .TestIsExists(
                    request.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var httpResponse = await client.PostFormData("/api/Feedback/SendFeedback", content, user);

            // Assert
            var message = await httpResponse.Content.ReadAsStringAsync();
            testLogger.WriteLine(message);
            //httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var addedFeedback = await context.Feedbacks
                .FirstOrDefaultAsync(x => 
                x.TestId == 1 && x.OwnerId == profile.Id);
        
            addedFeedback.Should().NotBeNull();
            addedFeedback.Rating.Should().Be(request.Rating);
            addedFeedback.OwnerId.Should().Be(profile.Id);
            addedFeedback.Text.Should().Be(request.Text);
            addedFeedback.TestId.Should().Be(request.TestId);
        }

        [Fact]
        public async Task DeleteFeedback_WhenUserIsntOwner_ShouldReturns409Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profile.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();
            var feedback = feedbackFromDb.Entity;

            var request = new DeleteFeedbackRequest
            {
                FeedbackId = feedback.Id,
            };

            var profileUserEntity = Profile.Initialize("testUser@mail.com", "test").Value;

            await context.Profiles.AddAsync(profileUserEntity);
            await context.SaveChangesAsync();

            var user = new JwtUserData
            {
                Email = "testUser@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Feedback/DeleteFeedback", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteFeedback_WhenUserIsAdmin_ShouldReturns204Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("text", 1, 
                5, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var request = new DeleteFeedbackRequest
            {
                FeedbackId = feedbackFromDb.Entity.Id,
            };

            var profileUserEntity = Profile.Initialize("testUser@mail.com", "test").Value;

            await context.Profiles.AddAsync(profileUserEntity);
            await context.SaveChangesAsync();

            var user = new JwtUserData
            {
                Email = "testUser@mail.com",
                Name = "test",
                Role = "Admin"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Feedback/DeleteFeedback", request, user);

            // Assert
            testLogger.WriteLine(await httpResponse.Content.ReadAsStringAsync());
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedFeedback = await context.Feedbacks
               .FirstOrDefaultAsync(x => x.Id == feedbackFromDb.Entity.Id);

            deletedFeedback.Should().BeNull();
        }

        [Fact]
        public async Task DeleteFeedback_WhenFeedbackDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var request = new DeleteFeedbackRequest
            {
                FeedbackId = 1,
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Feedback/DeleteFeedback", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteFeedback_WhenFeedbackExist_ShouldReturns204Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var feedbackEntity = Feedback.Initialize("text", 1, 5, profile.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();
            var feedback = feedbackFromDb.Entity;

            var request = new DeleteFeedbackRequest
            {
                FeedbackId = feedback.Id,
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Feedback/DeleteFeedback", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedFeedback = await context.Feedbacks
                .FirstOrDefaultAsync(x => x.Id == feedback.Id);
        
            deletedFeedback.Should().BeNull();
        }

        [Fact]
        public async Task ChangeFeedback_WhenFeedbackDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var request = new ChangeFeedbackRequest
            {
                FeedbackId = 1,
                Rating = 3,
                Text = "new text"
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Rating.ToString()), "Rating" },
                { new StringContent(request.FeedbackId.ToString()), "FeedbackId" },
                { new StringContent(request.Text), "Text" }
            };

            // Act
            var httpResponse = await client.PatchFormData("/api/Feedback/ChangeFeedback", content, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ChangeFeedback_WhenNotOwnerChanging_ShouldReturns409Status()
        {
            // Arrange
            var user = new JwtUserData
            {
                Email = "test123@mail.com",
                Name = "test",
                Role = "User"
            };

            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;
            var notOwnerProfileEntity = Profile.Initialize("test123@mail.com", "test").Value;

            await context.Profiles.AddAsync(notOwnerProfileEntity);
            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var feedbackEntity = Feedback.Initialize("olds text", 1, 9, profile.Id).Value;

            var feedbackFromDb = await context.Feedbacks
                .AddAsync(feedbackEntity);
            await context.SaveChangesAsync();
            var feedback = feedbackFromDb.Entity;

            var request = new ChangeFeedbackRequest
            {
                FeedbackId = feedback.Id,
                Rating = 3,
                Text = "new text"
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Rating.ToString()), "Rating" },
                { new StringContent(request.FeedbackId.ToString()), "FeedbackId" },
                { new StringContent(request.Text), "Text" }
            };

            // Act
            var httpResponse = await client.PatchFormData("/api/Feedback/ChangeFeedback", content, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ChangeFeedback_WhenAdminChanging_ShouldReturns204Status()
        {
            // Arrange
            var user = new JwtUserData
            {
                Email = "test123@mail.com",
                Name = "test",
                Role = "Admin"
            };

            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;
            var adminProfileEntity = Profile.Initialize("test123@mail.com", "test").Value;

            await context.Profiles.AddAsync(adminProfileEntity);
            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();

            var feedbackEntity = Feedback.Initialize("olds text", 1, 
                9, profileFromDb.Entity.Id).Value;

            var feedbackFromDb = await context.Feedbacks
                .AddAsync(feedbackEntity);
            await context.SaveChangesAsync();

            var request = new ChangeFeedbackRequest
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                Rating = 3,
                Text = "new text"
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Rating.ToString()), "Rating" },
                { new StringContent(request.FeedbackId.ToString()), "FeedbackId" },
                { new StringContent(request.Text), "Text" }
            };

            // Act
            var httpResponse = await client.PatchFormData("/api/Feedback/ChangeFeedback", content, user);

            // Assert
            testLogger.WriteLine(await httpResponse.Content.ReadAsStringAsync());
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            await massTransitHarness.InactivityTask;

            using var scope = serviceProvider.CreateScope();
            using var newContxet = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var newFeedback = await newContxet.Feedbacks
                .FirstOrDefaultAsync(x => x.Id == feedbackFromDb.Entity.Id);
            newFeedback.Should().NotBeNull();
            newFeedback.Text.Should().Be(request.Text);
            newFeedback.Rating.Should().Be(request.Rating);
        }

        [Fact]
        public async Task SendReview_WhenFeedbacExistAndReviewIsnt_ShouldReturns200Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;
            
            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var feedbackEntity = Feedback.Initialize("some text here", 1, 5, profile.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();
            var feedback = feedbackFromDb.Entity;

            var request = new SendReviewOnFeedbackRequest
            {
                FeedbackId = feedback.Id,
                IsPositive = false
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Feedback/SendReview", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var review = await context.Reviews
                .SingleOrDefaultAsync();

            review.Should().NotBeNull();
            review.ReviewedFeedbackId.Should().Be(request.FeedbackId);
            review.IsPositive.Should().Be(request.IsPositive);
        }

        [Fact]
        public async Task SendReview_WhenFeedbacDoesntExist_ShouldReturns400Status()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var request = new SendReviewOnFeedbackRequest
            {
                FeedbackId = 1,
                IsPositive = false
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Feedback/SendReview", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendReview_WhenSendSameReview_ShouldReturns200StatusAndDeleteOldReview()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var feedbackEntity = Feedback.Initialize("some text here", 1, 5, profile.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();
            var feedback = feedbackFromDb.Entity;

            var review = FeedbackReview.Initialize(false, profile.Id, feedback.Id).Value;

            var reviewFromDb = await context.Reviews.AddAsync(review);
            await context.SaveChangesAsync();

            var request = new SendReviewOnFeedbackRequest
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                IsPositive = false
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Feedback/SendReview", request, user);

            // Assert
            testLogger.WriteLine(await httpResponse.Content.ReadAsStringAsync());
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var deletedReview = await context.Reviews
                .FirstOrDefaultAsync(x => x.Id == review.Id);
            deletedReview.Should().BeNull();
        }

        [Fact]
        public async Task SendReview_WhenSendDifReview_ShouldReturns200StatusAndUpdateOldReview()
        {
            // Arrange
            var profileEntity = Profile.Initialize("test@mail.com", "test").Value;

            var profileFromDb = await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
            var profile = profileFromDb.Entity;

            var feedbackEntity = Feedback.Initialize("some text here", 1, 5, profile.Id).Value;

            var feedbackFromDb = await context.Feedbacks.AddAsync(feedbackEntity);
            await context.SaveChangesAsync();
            var feedback = feedbackFromDb.Entity;

            var reviewEntity = FeedbackReview.Initialize(false, profile.Id, feedback.Id).Value;

            var reviewFromDb = await context.Reviews.AddAsync(reviewEntity);
            await context.SaveChangesAsync();
            var review = reviewFromDb.Entity;

            var request = new SendReviewOnFeedbackRequest
            {
                FeedbackId = feedbackFromDb.Entity.Id,
                IsPositive = true
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PostAsUserAsync("/api/Feedback/SendReview", request, user);

            // Assert
            testLogger.WriteLine(await httpResponse.Content.ReadAsStringAsync());
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            using var scope = serviceProvider.CreateScope();
            using var newContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var updatedReview = await newContext.Reviews
                .FirstOrDefaultAsync(x => review.Id == x.Id);
            updatedReview.Should().NotBeNull();
            updatedReview.IsPositive.Should().Be(request.IsPositive);
        }
    }
}
