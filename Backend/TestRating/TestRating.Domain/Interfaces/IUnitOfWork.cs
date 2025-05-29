using System.Data;
using TestRating.Domain.Primitives;
using TestRating.Domain.Repositories;

namespace TestRating.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IFeedbackReportRepository ReportRepository { get; }

        IFeedbackRepository FeedbackRepository { get; }

        IFeedbackReviewRepository ReviewRepository { get; }

        IProfileRepository ProfileRepository { get; }

        IFeedbackReplyRepository ReplyRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IDatabaseTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        Task<IDatabaseTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

        void CommitTransaction(IDatabaseTransaction transaction);

        Task CommitTransactionAsync(IDatabaseTransaction transaction, CancellationToken cancellationToken = default);

        void RollBackTransaction(IDatabaseTransaction transaction);

        Task RollBackTransactionAsync(IDatabaseTransaction transaction, CancellationToken cancellationToken = default);

        bool CanConnect();

        Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
    }
}
