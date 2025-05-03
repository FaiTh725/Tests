using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Domain.Repositories
{
    public interface IFeedbackReportRepository
    {
        Task<FeedbackReport> AddReport(FeedbackReport feedbackReport, CancellationToken cancellationToken = default);

        Task<FeedbackReport?> GetFeedbackReport(long id, CancellationToken cancellationToken = default);

        Task<FeedbackReport?> GetFeedbackReportByCriteria(Specification<FeedbackReport> specification, CancellationToken cancellationToken = default);

        Task UpdateFeedbackReport(long id, FeedbackReport updatedReport, CancellationToken cancellationToken = default);
    }
}
