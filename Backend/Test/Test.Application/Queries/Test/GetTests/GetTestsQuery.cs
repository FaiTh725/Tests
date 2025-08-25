using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.Test.GetTests
{
    public class GetTestsQuery : 
        IRequest<PaginationResponse<TestInfo>>,
        ICachedData
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string Key => $"TestsInfo:{Page}-{PageSize}";

        public TimeSpan LifeTime => TimeSpan.FromSeconds(60);
    }
}
