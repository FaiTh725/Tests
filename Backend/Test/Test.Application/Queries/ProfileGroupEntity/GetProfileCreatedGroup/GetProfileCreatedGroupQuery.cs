using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.ProfileGroupEntity;

namespace Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup
{
    public class GetProfileCreatedGroupQuery : 
        IRequest<PaginationResponse<GroupWithMembers>>
    {
        public string ProfileEmail { get; set; } = string.Empty;

        public int Page {  get; set; }

        public int PageSize { get; set; }
    }
}
