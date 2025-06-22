using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.ProfileGroupEntity.Specifications
{
    public class GroupsByFirstLetterNameSpecification : 
        BaseSpecification<ProfileGroup>
    {
        public GroupsByFirstLetterNameSpecification(
            string groupName)
        {
            var groupNameLower = groupName.ToLower();

            Criteria = group => group.GroupName.ToLower()
                .StartsWith(groupNameLower);
        }
    }
}
