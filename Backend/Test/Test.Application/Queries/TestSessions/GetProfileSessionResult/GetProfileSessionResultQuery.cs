using MediatR;
using Test.Application.Contracts.TestSession;

namespace Test.Application.Queries.TestSessions.GetProfileSessionResult
{
    public class GetProfileSessionResultQuery : 
        IRequest<SessionResult>
    {
        public long Id { get; set; }
    }
}
