namespace TestRating.Domain.Primitives
{
    public interface IDatabaseTransaction : IDisposable
    {
        bool IsInTransaction { get; }
    }
}
