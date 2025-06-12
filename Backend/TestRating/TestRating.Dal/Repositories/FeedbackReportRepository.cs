using Microsoft.EntityFrameworkCore;
using TestRating.Dal.Specification;
using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;
using TestRating.Domain.Repositories;

namespace TestRating.Dal.Repositories
{
    public class FeedbackReportRepository : IFeedbackReportRepository
    {
        private readonly AppDbContext context;

        public FeedbackReportRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<FeedbackReport> AddReport(
            FeedbackReport feedbackReport, 
            CancellationToken cancellationToken = default)
        {
            var reportEntity = await context.Reports
                .AddAsync(feedbackReport, cancellationToken);

            return reportEntity.Entity;
        }

        public async Task<FeedbackReport?> GetFeedbackReport(
            long id, 
            CancellationToken cancellationToken = default)
        {
            return await context.Reports
                .FirstOrDefaultAsync(r => 
                r.Id == id, 
                cancellationToken);
        }

        public async Task<FeedbackReport?> GetFeedbackReportByCriteria(
            Specification<FeedbackReport> specification, 
            CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(
                context.Reports,
                specification)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateFeedbackReport(
            long id, 
            FeedbackReport updatedReport, 
            CancellationToken cancellationToken = default)
        {
            await context.Reports
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(x => x.ReportMessage, updatedReport.ReportMessage)
                    .SetProperty(x => x.IsApproval, updatedReport.IsApproval),
                    cancellationToken);
        }
    }
}
