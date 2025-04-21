using Test.Domain.Primitives;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Domain.Repositories
{
    public interface ITestRepository
    {
        Task<TestEntity> AddTest(TestEntity test, CancellationToken cancellationToken = default);

        Task<TestEntity?> GetTest(long id, CancellationToken cancellationToken = default);

        Task DeleteTest(long id, CancellationToken cancellationToken = default);

        Task UpdateTest(long id, TestEntity updatedTest, CancellationToken cancellationToken = default);

        Task<TestEntity?> GetTestByCriteria(BaseSpecification<TestEntity> specification, CancellationToken cancellationToken);

        Task<IEnumerable<TestEntity>> GetTestsByCriteria(BaseSpecification<TestEntity> specification, CancellationToken cancellationToken);
    }
}
