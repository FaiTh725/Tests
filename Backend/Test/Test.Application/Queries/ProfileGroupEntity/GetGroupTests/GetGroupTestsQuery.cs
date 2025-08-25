using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.ProfileGroupEntity.GetGroupTests
{
    public class GetGroupTestsQuery : 
        IRequest<PaginationResponse<TestInfo>>,
        ICachedData
    {
        public long GroupId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string Key => $"GroupTests:{GroupId}:{Page}-{PageSize}";

        public TimeSpan LifeTime => TimeSpan.FromSeconds(120);
    }
}
