using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Domain.Enums;

namespace Test.Application.Commands.TestAccessEntity.LimitTestAccess
{
    public class LimitTestAccessCommand :
        IRequest,
        IOwnerAndAdminTestAccess
    {
        public long TargetEntityId { get; set; }

        public TargetAccessEntityType TargetEntity {  get; set; }

        public long TestId { get; set; }
        
        public string Role { get; set; } = string.Empty;

        public long OwnerId { get; set; }
    }
}
