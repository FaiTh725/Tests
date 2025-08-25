using FluentAssertions;
using MongoDB.Driver;
using Notification.API.Contracts.Notifications;
using Notification.Infrastructure.Data.Persistences;
using Notification.IntegrationTests.Extensions;
using Notification.IntegrationTests.JwtToken;
using System.Net;

namespace Notification.IntegrationTests.API.Controllers
{
    [Collection("Integration Tests")]
    public class NotificationControllerTests : 
        BaseIntegrationTest
    {
        public NotificationControllerTests(
            CustomWebFactory factory) : base(factory)
        {}

        [Fact]
        public async Task ReadAllNotifications_WhenUserIsUnauthorized_ShouldReturnsStatus401()
        {
            // Arrange
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri("/api/Notification/ReadAllNotifications", UriKind.Relative)
            };

            // Act
            var httpResponse = await client.SendAsync(httpRequest);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ReadAllNotifications_ReturnsStatus204ReadAllUnreadNotification()
        {
            // Arrange
            var existedUnreadNotification = new MongoNotification
            {
                Id = 1,
                IsRead = false,
                Message = "some message here",
                SendTime = new DateTime(2025, 5, 10),
                Title = "title",
                UserEmail = "test@mail.com"
            };

            await context.Notifications.InsertOneAsync(existedUnreadNotification);
            
            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Notification/ReadAllNotifications", user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updatedNotification = await context.Notifications
                .Find(x => x.Id == existedUnreadNotification.Id)
                .FirstOrDefaultAsync();

            updatedNotification.Should().NotBeNull();
            updatedNotification.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task ReadNotification_WhenNotificationDoesntExist_ShouldReturnsStatus400()
        {
            // Arrange
            var request = new ReadNotificationRequest
            {
                NotificationId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Notification/ReadNotification", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ReadNotification_WhenNotificationRecipientIsntSender_ShouldReturnsStatus409()
        {
            // Arrange
            var existedNotification = new MongoNotification
            {
                Id = 1,
                IsRead = false,
                Message = "message",
                SendTime = new DateTime(2025, 5, 10),
                Title = "title",
                UserEmail = "notsender@mail.com"
            };

            await context.Notifications
                .InsertOneAsync(existedNotification);

            var request = new ReadNotificationRequest
            {
                NotificationId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Notification/ReadNotification", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task ReadNotification_WhenRequestIsCorrect_ShouldReturnsStatus204AndMarkNotificationAsRead()
        {
            // Arrange
            var existedNotification = new MongoNotification
            {
                Id = 1,
                IsRead = false,
                Message = "message",
                SendTime = new DateTime(2025, 5, 10),
                Title = "title",
                UserEmail = "test@mail.com"
            };

            await context.Notifications
                .InsertOneAsync(existedNotification);

            var request = new ReadNotificationRequest
            {
                NotificationId = 1
            };

            var user = new JwtUserData
            {
                Email = "test@mail.com",
                Name = "test",
                Role = "User"
            };

            // Act
            var httpResponse = await client.PathAsUserAsync("/api/Notification/ReadNotification", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var readNotification = await context.Notifications
                .Find(x => x.Id == request.NotificationId)
                .SingleOrDefaultAsync();
        
            readNotification.Should().NotBeNull();
            readNotification.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProfileNotifications_WhenNotificationsHasDifRecipient_ShouldReturnsStatus400()
        {
            // Arrange
            var existedNotificationOwnerIsSender = new MongoNotification()
            {
                Id = 1,
                IsRead = false,
                Message = "message",
                SendTime = new DateTime(2025, 5, 10),
                Title = "title",
                UserEmail = "test@mail.com"
            };
            var existedNotificationOwnerIsntSender = new MongoNotification()
            {
                Id = 2,
                IsRead = false,
                Message = "message",
                SendTime = new DateTime(2025, 5, 10),
                Title = "title",
                UserEmail = "isnttest@mail.com"
            };

            await context.Notifications.InsertManyAsync([
                existedNotificationOwnerIsSender, 
                existedNotificationOwnerIsntSender]);

            var request = new DeleteNotificationsRequest
            {
                NotificationsIdToDelete = [1, 2]
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Notification/DeleteProfileNotifications", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteProfileNotifications_WhenRequestIsCorrect_ShouldReturnsStatus204()
        {
            // Arrange
            var existedNotificationOwnerIsSender = new MongoNotification()
            {
                Id = 1,
                IsRead = false,
                Message = "message",
                SendTime = new DateTime(2025, 5, 10),
                Title = "title",
                UserEmail = "test@mail.com"
            };
            var existedNotificationOwnerIsntSender = new MongoNotification()
            {
                Id = 2,
                IsRead = false,
                Message = "message",
                SendTime = new DateTime(2025, 5, 10),
                Title = "title",
                UserEmail = "test@mail.com"
            };

            await context.Notifications.InsertManyAsync([
                existedNotificationOwnerIsSender,
                existedNotificationOwnerIsntSender]);

            var request = new DeleteNotificationsRequest
            {
                NotificationsIdToDelete = [1, 2]
            };

            var user = new JwtUserData
            {
                Name = "test",
                Email = "test@mail.com",
                Role = "User"
            };

            // Act
            var httpResponse = await client.DeleteAsUserAsync("/api/Notification/DeleteProfileNotifications", request, user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedNotifications = await context.Notifications
                .Find(x => request.NotificationsIdToDelete.Contains(x.Id))
                .ToListAsync();

            deletedNotifications.Should().BeEmpty();
        }
    }
}
