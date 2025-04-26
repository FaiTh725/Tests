using MediatR;
using Test.Application.Contracts.ProfileGroupEntity;

namespace Test.Application.Queries.ProfileGroupEntity.GetProfileJoinedGroup
{
    public class GetProfileJoinedGroupQuery :
        IRequest<IEnumerable<GroupInfo>>
    {
        public long ProfileId { get; set; }
    }
}
