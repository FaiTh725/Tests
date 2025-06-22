using Test.API.Contracts.Common;

namespace Test.API.Contracts.Group
{
    public class GetGroupTestsRequest : GetPaginatedDataRequest
    {
        public long GroupId { get; set; }
    }
}
