using FluentAssertions;
using Notification.Application.DTOs.Notifications;
using Notification.Application.Queries.Notifications.GetUnreadNotifications;
using Notification.Infrastructure.Data.Persistences;

namespace Notification.IntegrationTests.Application.Queries
{
    [Collection("Integration Tests")]
    public class GetUnreadNotificationsQueryHandlerTests : BaseIntegrationTest
    {
        public GetUnreadNotificationsQueryHandlerTests(
            CustomWebFactory factory) : base(factory)
        {}

        [Fact]
        public async Task Handle_ReturnOnlyUnreadUserNotifications()
        {
            // Arrange
            var existedReadNotification = new MongoNotification()
            {
                Id = 1,
                IsRead = true,
                Message = "message",
                SendTime = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                Title = "title",
                UserEmail = "test@mail.com"
            };
            var existedUnreadNotification = new MongoNotification()
            {
                Id = 2,
                IsRead = false,
                Message = "message",
                SendTime = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                Title = "title",
                UserEmail = "test@mail.com"
            };

            await context.Notifications
                .InsertManyAsync([existedReadNotification, existedUnreadNotification]);

            var query = new GetUnreadNotificationsQuery
            {
                UserEmail = "test@mail.com",
                Page = 1,
                PageSize = 12
            };

            var expectedResult = new List<NotificationDTO>
            {
                new NotificationDTO
                {
                    Id = 2,
                    ConsumerEmail = "test@mail.com",
                    Message = "message",
                    IsRead = false,
                    SendTime = new DateTime(2025, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                    Title = "title",
                }
            };

            // Act
            var unreadNotifications = await sender.Send(query, CancellationToken.None);

            // Assert
            unreadNotifications.Should().BeEquivalentTo(expectedResult);
        }
    }
}
