using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.Test;

namespace Test.Application.Queries.ProfileEntity.GetAvailableProfileTests
{
    public class GetAvailableProfileTestsQuery :
        IRequest<PaginationResponse<TestInfo>>,
        ICachedData
    {
        public long ProfileId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string Key => "ProfileAvailableTests:" + ProfileId;

        public TimeSpan LifeTime => TimeSpan.FromSeconds(120);
    }
}
