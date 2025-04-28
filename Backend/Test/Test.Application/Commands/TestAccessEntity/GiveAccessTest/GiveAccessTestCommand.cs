using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;
using Test.Domain.Enums;

namespace Test.Application.Commands.TestAccessEntity.GiveAccessTest
{
    public class GiveAccessTestCommand :
        IRequest<long>,
        IOwnerAndAdminTestAccess
    {
        public long AccessTargetEntityId {  get; set; }

        public TargetAccessEntityType TargetEntity { get; set; }

        public long TestId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
