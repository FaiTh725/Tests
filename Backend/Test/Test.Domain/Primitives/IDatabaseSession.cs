namespace Test.Domain.Primitives
{
    public interface IDatabaseSession : IDisposable
    {
        bool IsClosed { get; }
    }
}
