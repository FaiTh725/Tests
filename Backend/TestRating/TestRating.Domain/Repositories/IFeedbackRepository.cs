using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Domain.Repositories
{
    public interface IFeedbackRepository
    {
        Task<Feedback> AddFeedback(Feedback feedback, CancellationToken cancellationToken = default);

        Task<Feedback?> GetFeedbackById(long id, CancellationToken cancellationToken = default);
        
        Task<Feedback?> GetFeedbackExcludeFiltersById(long id, CancellationToken cancellationToken = default);

        Task<Feedback?> GetFeedbackByCriteria(Specification<Feedback> specification, CancellationToken cancellationToken = default);

        Task UpdateFeedback(long feedbackId, Feedback updatedFeedback, CancellationToken cancellationToken = default);

        Task SoftDeleteFeedback(long id, CancellationToken cancellationToken = default);

        Task HardDeleteFeedback(long id, CancellationToken cancellationToken = default);
    }
}
