using MediatR;
using Test.Application.Contracts.ProfileGroupEntity;

namespace Test.Application.Queries.ProfileGroupEntity.GetGroupsByFirstLettersName
{
    public class GetGroupsByFirstLettersNameQuery : 
        IRequest<IEnumerable<GroupInfo>>
    {
        public string GroupName { get; set; } = string.Empty;
    }
}
