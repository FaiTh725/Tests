using CSharpFunctionalExtensions;

namespace TestRating.Domain.Primitives
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; }

        DateTime? DeletedTime { get; }

        Result Delete();
    }
}
