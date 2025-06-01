namespace Notification.Domain.Primitives
{
    public interface IDatabaseTransaction : IDisposable
    {
        bool IsClosed { get; }
    }
}
