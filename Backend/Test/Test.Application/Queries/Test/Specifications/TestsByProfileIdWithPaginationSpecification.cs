using Test.Domain.Primitives;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Application.Queries.Test.Specifications
{
    public class TestsByProfileIdWithPaginationSpecification :
        BaseSpecification<TestEntity>
    {
        public TestsByProfileIdWithPaginationSpecification(
            long profileId,
            int page, 
            int pageSize)
        {
            Criteria = test => test.ProfileId == profileId;

            IsEnablePagination = true;
            Page = page;
            PageSize = pageSize;
        }
    }
}
