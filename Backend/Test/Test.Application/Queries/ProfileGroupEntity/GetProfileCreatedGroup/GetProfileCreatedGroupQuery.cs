using MediatR;
using Test.Application.Contracts.ProfileGroupEntity;

namespace Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup
{
    public class GetProfileCreatedGroupQuery : 
        IRequest<IEnumerable<GroupInfo>>
    {
        public long ProfileId { get; set; }
    }
}
