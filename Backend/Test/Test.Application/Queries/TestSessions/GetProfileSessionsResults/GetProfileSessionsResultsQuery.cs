using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.TestSession;

namespace Test.Application.Queries.TestSessions.GetProfileSessionsResults
{
    public class GetProfileSessionsResultsQuery : 
        IRequest<PaginationResponse<SessionResult>>,
        ICachedData
    {
        public long ProfileId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string Key => $"ProfileSessions:{ProfileId}:{Page}-{PageSize}";

        public TimeSpan LifeTime => TimeSpan.FromSeconds(30);
    }
}
