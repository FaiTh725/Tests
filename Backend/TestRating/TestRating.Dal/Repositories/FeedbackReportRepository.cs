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
    }
}
