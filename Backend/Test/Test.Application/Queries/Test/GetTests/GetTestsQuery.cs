using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.Test.GetTests
{
    public class GetTestsQuery : 
        IRequest<PaginationResponse<TestInfo>>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
