using MediatR;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.TestEntity.GetTestInfoById
{
    public class GetTestInfoByIdQuery : IRequest<TestInfo>
    {
        public long Id { get; set; }
    }
}
