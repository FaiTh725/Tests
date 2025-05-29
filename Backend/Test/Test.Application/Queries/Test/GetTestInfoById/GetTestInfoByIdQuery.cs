using MediatR;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.Test.GetTestInfoById
{
    public class GetTestInfoByIdQuery : IRequest<TestInfo>
    {
        public long Id { get; set; }
    }
}
