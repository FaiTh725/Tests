using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class TestSessionRepository : ITestSessionRepository
    {
        private readonly AppDbContext context;

        public TestSessionRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public Task<TestSession> AddTestSession(TestSession testSession, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestSession?> GetTestSession(long testSessionId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
