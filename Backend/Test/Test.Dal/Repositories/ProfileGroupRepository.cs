using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class ProfileGroupRepository : IProfileGroupRepository
    {
        private readonly AppDbContext context;

        public ProfileGroupRepository(
            AppDbContext context)
        {
            this.context = context;
        }
    }
}
