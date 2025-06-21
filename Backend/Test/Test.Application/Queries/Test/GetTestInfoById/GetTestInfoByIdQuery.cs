using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.Test.GetTestInfoById
{
    public class GetTestInfoByIdQuery : 
        IRequest<TestInfo>,
        ICachedData
    {
        public long Id { get; set; }

        public string Key => "TestInfo:" + Id;

        public TimeSpan LifeTime => TimeSpan.FromSeconds(180);
    }
}
