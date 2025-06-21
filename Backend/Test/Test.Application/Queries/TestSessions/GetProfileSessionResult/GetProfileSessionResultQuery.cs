using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Contracts.TestSession;

namespace Test.Application.Queries.TestSessions.GetProfileSessionResult
{
    public class GetProfileSessionResultQuery : 
        IRequest<SessionResult>,
        ICachedData
    {
        public long Id { get; set; }

        public string Key => "ProfileSession:" + Id;

        public TimeSpan LifeTime => TimeSpan.FromSeconds(120);
    }
}
