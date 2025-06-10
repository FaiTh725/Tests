using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.ProfileEntity.Specifications
{
    public class GetProfilesByEmailFirstLettersSpecification :
        BaseSpecification<Profile>
    {
        public GetProfilesByEmailFirstLettersSpecification(
            string email)
        {
            Criteria = profile => profile.Email.ToLower()
                .StartsWith(email.ToLower());
        }
    }
}
