using TestRating.Domain.Repositories;

namespace TestRating.Dal.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext context;

        public FeedbackRepository(
            AppDbContext context)
        {
            this.context = context;
        }
    }
}
