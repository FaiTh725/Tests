using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.ProfileGroupEntity.Specifications
{
    public class GroupsProfileJoinedSpecification :
        BaseSpecification<ProfileGroup>
    {
        public GroupsProfileJoinedSpecification(
            long profileId)
        {
            Criteria = group => group.MembersId.Any(x => x == profileId);
        }
    }
}
