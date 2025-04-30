using TestRating.Domain.Repositories;

namespace TestRating.Dal.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext context;

        public ProfileRepository(
            AppDbContext context)
        {
            this.context = context;
        }
    }
}
