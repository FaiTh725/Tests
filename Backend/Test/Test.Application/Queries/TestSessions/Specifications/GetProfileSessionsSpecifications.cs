using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.TestSessions.Specifications
{
    public class GetProfileSessionsSpecifications : 
        BaseSpecification<TestSession>
    {
        public GetProfileSessionsSpecifications(
            long profileId)
        {
            Criteria = session => session.ProfileId == profileId && session.IsEnded;
        }
    }
}
