using Test.Domain.Primitives;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Application.Queries.Test.Specifications
{
    public class TestsByProfileIdSpecification :
        BaseSpecification<TestEntity>
    {
        public TestsByProfileIdSpecification(
            long profileId)
        {
            Criteria = test => test.ProfileId == profileId;
        }
    }
}
