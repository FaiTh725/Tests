using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface ITestSessionRepository
    {
        Task<TestSession> AddTestSession(TestSession testSession, IDatabaseSession? session = null, CancellationToken cancellationToken = default);

        Task<TestSession?> GetTestSession(long testSessionId, CancellationToken cancellationToken = default);

        Task UpdateTestSession(long id, TestSession updatedSession, IDatabaseSession? session = null, CancellationToken cancellationToken = default);
    }
}
