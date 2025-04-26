using MediatR;
using Test.Application.Contracts.ProfileGroupEntity;

namespace Test.Application.Queries.ProfileGroupEntity.GetGroupById
{
    public class GetGroupByIdQuery : 
        IRequest<GroupInfoWithOwner>
    {
        public long Id { get; set; }
    }
}
