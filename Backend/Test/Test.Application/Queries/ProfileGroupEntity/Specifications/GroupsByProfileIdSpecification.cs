using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.ProfileGroupEntity.Specifications
{
    public class GroupsByProfileIdSpecification : 
        BaseSpecification<ProfileGroup>
    {
        public GroupsByProfileIdSpecification(
            long profileId)
        {
            Criteria = group => group.OwnerId == profileId;
        }
    }
}
