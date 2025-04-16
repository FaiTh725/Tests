using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class TestRepository : ITestRepository
    {
        public Task<Domain.Entities.Test> AddTest(Domain.Entities.Test test, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Test?> GetTest(long id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
