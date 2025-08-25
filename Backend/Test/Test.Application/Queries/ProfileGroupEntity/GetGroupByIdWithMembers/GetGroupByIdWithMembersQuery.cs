using MediatR;
using Test.Application.Contracts.ProfileGroupEntity;

namespace Test.Application.Queries.ProfileGroupEntity.GetGroupByIdWithMembers
{
    public class GetGroupByIdWithMembersQuery : 
        IRequest<GroupWithMembers>
    {
        public long GroupId {  get; set; }
    }
}
