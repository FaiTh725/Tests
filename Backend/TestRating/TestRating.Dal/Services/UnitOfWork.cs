using Microsoft.EntityFrameworkCore;
using System.Data;
using TestRating.Dal.Adapters;
using TestRating.Dal.Repositories;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Primitives;
using TestRating.Domain.Repositories;

namespace TestRating.Dal.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;

        private bool disposed = false;

        private readonly Lazy<IFeedbackRepository> feedbackRepository;
        private readonly Lazy<IFeedbackReportRepository> reportRepository;
        private readonly Lazy<IFeedbackReviewRepository> reviewRepository;
        private readonly Lazy<IProfileRepository> profileRepository;
        private readonly Lazy<IFeedbackReplyRepository> replyRepository;

        public UnitOfWork(
            AppDbContext context)
        {
            this.context = context;

            feedbackRepository = new Lazy<IFeedbackRepository>(() => new FeedbackRepository(context));
            reportRepository = new Lazy<IFeedbackReportRepository>(() => new FeedbackReportRepository(context));
            reviewRepository = new Lazy<IFeedbackReviewRepository>(() => new FeedbackReviewRepository(context));
            profileRepository = new Lazy<IProfileRepository>(() => new ProfileRepository(context));
            replyRepository = new Lazy<IFeedbackReplyRepository>(() => new FeedbackReplyRepository(context));
        }

        public IFeedbackReportRepository ReportRepository => reportRepository.Value;

        public IFeedbackRepository FeedbackRepository => feedbackRepository.Value;

        public IFeedbackReviewRepository ReviewRepository => reviewRepository.Value;

        public IProfileRepository ProfileRepository => profileRepository.Value;

        public IFeedbackReplyRepository ReplyRepository => replyRepository.Value;

        public IDatabaseTransaction BeginTransaction(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var transaction = context.Database
                .BeginTransaction(isolationLevel);

            return new DbContextTransactionAdapter(transaction);
        }

        public async Task<IDatabaseTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, 
            CancellationToken cancellationToken = default)
        {
            var transaction = await context.Database
                .BeginTransactionAsync(cancellationToken);

            return new DbContextTransactionAdapter(transaction);
        }

        public bool CanConnect()
        {
            return context.Database.CanConnect();
        }

        public async Task<bool> CanConnectAsync(
            CancellationToken cancellationToken = default)
        {
            return await context.Database
                .CanConnectAsync(cancellationToken);
        }

        public void CommitTransaction(IDatabaseTransaction transaction)
        {
            var dbTransaction = transaction as DbContextTransactionAdapter;
            AssuranceTransaction(dbTransaction);

            dbTransaction!.Transaction.Commit();
        }

        public async Task CommitTransactionAsync(
            IDatabaseTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var dbTransaction = transaction as DbContextTransactionAdapter;
            AssuranceTransaction(dbTransaction);

            await dbTransaction!.Transaction
                .CommitAsync(cancellationToken);
        }

        public void RollBackTransaction(
            IDatabaseTransaction transaction)
        {
            var dbTransaction = transaction as DbContextTransactionAdapter;
            AssuranceTransaction(dbTransaction);

            dbTransaction!.Transaction.Rollback();
        }

        public async Task RollBackTransactionAsync(
            IDatabaseTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var dbTransaction = transaction as DbContextTransactionAdapter;
            AssuranceTransaction(dbTransaction);

            await dbTransaction!.Transaction
                .RollbackAsync(cancellationToken);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void AssuranceTransaction(IDatabaseTransaction? transaction)
        {
            if (transaction is not null && 
                !transaction.IsInTransaction)
            {
                throw new InvalidOperationException("Transaction isnt started");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                context.Dispose();
            }
            disposed = true;
        }
    }
}
