namespace Authorization.Domain.Primitives
{
    public interface IDatabaseTransaction : IDisposable
    {
        bool IsInTransaction { get; }
    }
}
