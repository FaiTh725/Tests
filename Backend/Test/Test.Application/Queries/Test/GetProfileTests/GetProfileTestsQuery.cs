using MediatR;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.Test.GetProfileTests
{
    public class GetProfileTestsQuery : 
        IRequest<IEnumerable<TestInfo>>
    {
        public long ProfileId { get; set; }
    }
}
