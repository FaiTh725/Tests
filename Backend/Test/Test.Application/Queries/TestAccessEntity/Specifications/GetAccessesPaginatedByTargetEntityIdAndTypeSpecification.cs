using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Primitives;

namespace Test.Application.Queries.TestAccessEntity.Specifications
{
    public class GetAccessesPaginatedByTargetEntityIdAndTypeSpecification : 
        BaseSpecification<TestAccess>
    {
        public GetAccessesPaginatedByTargetEntityIdAndTypeSpecification(
            long targetEntityId, TargetAccessEntityType entityType,
            int page, int pageSize)
        {
            Criteria = access => access.TargetEntityId == targetEntityId &&
                access.TargetAccessEntityType == entityType;

            IsEnablePagination = true;
            Page = page;
            PageSize = pageSize;
        }
    }
}
