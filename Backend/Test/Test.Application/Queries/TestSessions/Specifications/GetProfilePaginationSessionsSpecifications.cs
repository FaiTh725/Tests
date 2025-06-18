using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.TestSessions.Specifications
{
    public class GetProfilePaginationSessionsSpecifications : 
        BaseSpecification<TestSession>
    {
        public GetProfilePaginationSessionsSpecifications(
            long profileId, int page, int pageSize)
        {
            Criteria = session => session.ProfileId == profileId && session.IsEnded;


            IsEnablePagination = true;
            Page = page;
            PageSize = pageSize;
        }
    }
}
