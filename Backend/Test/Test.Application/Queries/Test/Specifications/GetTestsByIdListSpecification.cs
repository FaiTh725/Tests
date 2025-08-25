using Test.Domain.Primitives;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Application.Queries.Test.Specifications
{
    public class GetTestsByIdListSpecification : BaseSpecification<TestEntity>
    {
        public GetTestsByIdListSpecification(
            List<long> testIds)
        {
            Criteria = test => testIds.Contains(test.Id);
        }
    }
}
