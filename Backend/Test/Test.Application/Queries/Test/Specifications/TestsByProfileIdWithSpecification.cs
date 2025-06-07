using Test.Domain.Primitives;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Application.Queries.Test.Specifications
{
    public class TestsByProfileIdWithSpecification : 
        BaseSpecification<TestEntity>
    {
        public TestsByProfileIdWithSpecification(
            long profileId)
        {
            Criteria = test => test.ProfileId == profileId;
        }
    }
}
