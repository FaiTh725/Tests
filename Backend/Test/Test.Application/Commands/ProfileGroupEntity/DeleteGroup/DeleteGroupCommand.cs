using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;

namespace Test.Application.Commands.ProfileGroupEntity.DeleteGroup
{
    public class DeleteGroupCommand :
        IRequest,
        IOwnerAndAdminGroupAccess
    {
        public long OwnerId { get; set; }

        public long GroupId { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}
