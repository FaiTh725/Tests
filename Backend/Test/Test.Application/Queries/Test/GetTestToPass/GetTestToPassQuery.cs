using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.Test.GetTestToPass
{
    public class GetTestToPassQuery : 
        IRequest<TestToPassResponse>,
        ICachedData
    {
        public long Id { get; set; }

        public string Key => "TestToPass:" + Id;

        public TimeSpan LifeTime => TimeSpan.FromSeconds(180);
    }
}
