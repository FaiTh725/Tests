using TestRating.Domain.Repositories;

namespace TestRating.Dal.Repositories
{
    public class FeedbackReviewRepository : IFeedbackReviewRepository
    {
        private readonly AppDbContext context;

        public FeedbackReviewRepository(
            AppDbContext context)
        {
            this.context = context;
        }
    }
}
