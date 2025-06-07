using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.Test.GetProfileTests
{
    public class GetProfileTestsQuery : 
        IRequest<PaginationResponse<TestInfo>>
    {
        public string ProfileEmail { get; set; } = string.Empty;

        public int Page {  get; set; }

        public int PageCount { get; set; }
    }
}
