using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class ProfileAnswerRepository : IProfileAnswerRepository
    {
        private readonly AppDbContext context;

        public ProfileAnswerRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public Task<IEnumerable<ProfileAnswer>> AddProfileAnswers(List<ProfileAnswer> profileAnswers, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
