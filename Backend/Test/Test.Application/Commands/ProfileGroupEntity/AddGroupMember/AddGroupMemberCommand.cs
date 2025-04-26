using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;

namespace Test.Application.Commands.ProfileGroupEntity.AddGroupMember
{
    public class AddGroupMemberCommand : 
        IRequest,
        IOwnerAndAdminGroupAccess
    {
        public long GroupId { get; set; }

        public long ProfileId { get; set; }

        public long OwnerId { get ; set; }

        public string Role { get; set; } = string.Empty;
    }
}
