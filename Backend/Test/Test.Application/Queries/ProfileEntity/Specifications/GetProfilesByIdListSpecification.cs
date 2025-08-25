using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.ProfileEntity.Specifications
{
    public class GetProfilesByIdListSpecification : 
        BaseSpecification<Profile>
    {
        public GetProfilesByIdListSpecification(
            List<long> profilesId)
        {
            Criteria = profile => profilesId.Contains(profile.Id);
        }
    }
}
