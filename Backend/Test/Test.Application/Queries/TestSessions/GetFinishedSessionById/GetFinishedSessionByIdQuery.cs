using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Contracts.TestSession;

namespace Test.Application.Queries.TestSessions.GetFinishedSessionById
{
    public class GetFinishedSessionByIdQuery : 
        IRequest<SessionInfo>,
        ICachedData
    {
        public long Id { get; set; }

        public string Key => "FinishedSession:" + Id;

        public TimeSpan LifeTime => TimeSpan.FromSeconds(30);
    }
}
