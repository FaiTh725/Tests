namespace Test.API.Hubs.Interfaces
{
    public interface ITestSessionHub
    {
        Task TestStoped(long sessionId);
    }
}
