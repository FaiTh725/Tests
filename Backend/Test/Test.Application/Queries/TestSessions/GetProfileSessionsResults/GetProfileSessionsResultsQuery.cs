using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.TestSession;

namespace Test.Application.Queries.TestSessions.GetProfileSessionsResults
{
    public class GetProfileSessionsResultsQuery : 
        IRequest<PaginationResponse<SessionResult>>
    {
        public long ProfileId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
