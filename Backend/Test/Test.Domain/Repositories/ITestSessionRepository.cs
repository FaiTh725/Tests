using Test.Domain.Entities;

namespace Test.Domain.Repositories
{
    public interface ITestSessionRepository
    {
        Task<TestSession> AddTestSession(TestSession testSession, CancellationToken cancellationToken = default);

        Task<TestSession?> GetTestSession(long testSessionId, CancellationToken cancellationToken = default);

        Task UpdateTestSession(long id, TestSession updatedSession, CancellationToken cancellationToken = default);
    }
}
