using MediatR;
using Test.Application.Common.BehaviorsInterfaces;

namespace Test.Application.Commands.ProfileGroupEntity.DeleteMembersGroup
{
    public class DeleteMembersGroupCommand :
        IRequest,
        IOwnerAndAdminGroupAccess
    {
        public List<long> MembersId { get; set; } = new List<long>();
        
        public long GroupId { get; set; }
        
        public string Role { get; set; } = string.Empty;
        
        public long OwnerId { get; set; }
    }
}
