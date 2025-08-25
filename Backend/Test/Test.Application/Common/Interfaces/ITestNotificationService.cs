namespace Test.Application.Common.Interfaces
{
    public interface ITestNotificationService
    {
        Task NotifyTestOver(string userEmail, long sessionId);
    }
}
