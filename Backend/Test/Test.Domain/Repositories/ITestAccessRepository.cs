using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface ITestAccessRepository
    {
        Task<TestAccess> AddTestAccess(TestAccess testAccess, CancellationToken cancellationToken = default);

        Task<TestAccess?> GetTestAccess(long testId, long targetEntityId, TargetAccessEntityType entityType, CancellationToken cancellationToken = default);
        
        Task<TestAccess?> GetTestAccess(long accessId, CancellationToken cancellationToken = default);

        Task DeleteTestAccess(long testAccess, CancellationToken cancellationToken = default);

        Task<IEnumerable<TestAccess>> GetAccessesByCriteria(BaseSpecification<TestAccess> specification, CancellationToken cancellationToken = default);
    }
}
