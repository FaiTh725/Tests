using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.ProfileGroupEntity.Specifications
{
    public class GroupsProfileJoinedPaginationSpecification :
        BaseSpecification<ProfileGroup>
    {
        public GroupsProfileJoinedPaginationSpecification(
            long profileId, 
            int page, 
            int pageSize)
        {
            Criteria = group => group.MembersId.Any(x => x == profileId);
        
            IsEnablePagination = true;
            Page = page;
            PageSize = pageSize;
        }
    }
}
