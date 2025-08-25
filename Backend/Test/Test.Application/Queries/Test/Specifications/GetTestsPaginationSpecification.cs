using Test.Domain.Primitives;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Application.Queries.Test.Specifications
{
    public class GetTestsPaginationSpecification : 
        BaseSpecification<TestEntity>
    {
        public GetTestsPaginationSpecification(
            int page, int pageSize)
        {
            IsEnablePagination = true;
            Page = page;
            PageSize = pageSize;
        }
    }
}
