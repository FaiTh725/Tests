using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.ProfileGroupEntity.Specifications
{
    public class GroupsByProfileIdPaginationSpecification : 
        BaseSpecification<ProfileGroup>
    {
        public GroupsByProfileIdPaginationSpecification(
            long profileId,
            int page, 
            int pageSize)
        {
            Criteria = group => group.OwnerId == profileId;

            IsEnablePagination = true;
            Page = page;
            PageSize = pageSize;
        }
    }
}
