using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using TestRating.Dal.Repositories;
using TestRating.Domain.Interfaces;
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

        private IDbContextTransaction transaction;

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

        public void BeginTransaction(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            transaction = context.Database
                .BeginTransaction(isolationLevel);
        }

        public async Task BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, 
            CancellationToken cancellationToken = default)
        {
            transaction = await context.Database
                .BeginTransactionAsync(cancellationToken);
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

        public void CommitTransaction()
        {
            AssuranceTransaction();

            transaction.Commit();
            transaction.Dispose();
        }

        public async Task CommitTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            AssuranceTransaction();

            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
        }

        public void RollBackTransaction()
        {
            AssuranceTransaction();

            transaction.Rollback();
            transaction.Dispose();
        }

        public async Task RollBackTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            AssuranceTransaction();

            await transaction.RollbackAsync(cancellationToken);
            await transaction.DisposeAsync();
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
            GC.SuppressFinalize(this);
        }

        private void AssuranceTransaction()
        {
            if (transaction is null)
            {
                throw new InvalidOperationException("Transaction hasnt been started");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                context.Dispose();
                transaction?.Dispose();
            }
            disposed = true;
        }
    }
}
