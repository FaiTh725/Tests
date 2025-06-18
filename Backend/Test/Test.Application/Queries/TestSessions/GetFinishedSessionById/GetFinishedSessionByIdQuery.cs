using MediatR;
using Test.Application.Contracts.TestSession;

namespace Test.Application.Queries.TestSessions.GetFinishedSessionById
{
    public class GetFinishedSessionByIdQuery : 
        IRequest<SessionInfo>
    {
        public long Id { get; set; }
    }
}
