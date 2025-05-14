using MediatR;
using Test.Application.Common.BehaviorsInterfaces;

namespace Test.Application.Commands.ProfileGroupEntity.DeleteGroup
{
    public class DeleteGroupCommand :
        IRequest,
        IOwnerAndAdminGroupAccess
    {
        public long GroupId { get; set; }

        public string Role { get; set; } = string.Empty;

        public string OwnerEmail { get; set; } = string.Empty;
    }
}
