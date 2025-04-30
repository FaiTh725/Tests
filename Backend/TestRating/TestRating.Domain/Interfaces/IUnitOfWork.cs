using TestRating.Domain.Repositories;

namespace TestRating.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IFeedbackReportRepository ReportRepository { get; }

        IFeedbackRepository FeedbackRepository { get; }

        IFeedbackReviewRepository ReviewRepository { get; }

        IProfileRepository ProfileRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void BeginTransaction();

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        void CommitTransaction();

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        void RollBackTransaction();

        Task RollBackTransactionAsync(CancellationToken cancellationToken = default);

        bool CanConnect();

        Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
    }
}
