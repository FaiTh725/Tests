using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.TestAccessEntity.Specifications
{
    public class AccessesByTargetEntitySpecification :
        BaseSpecification<TestAccess>
    {
        public AccessesByTargetEntitySpecification(
            List<long> profilesId)
        {
            var profilesIdHashSet = profilesId.ToHashSet();
            Criteria = access => profilesIdHashSet.Contains(access.TargetEntityId);
        }
    }
}
