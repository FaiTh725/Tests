using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;
using Test.Domain.Enums;

namespace Test.Application.Commands.Test.UpdateTest
{
    public class UpdateTestCommand : 
        IRequest<long>,
        IOwnerAndAdminTestAccess
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsPublic { get; set; }

        public TestType TestType { get; set; }

        public double? DurationInMinutes { get; set; }

        public long TestId { get; set; }

        public string Email { get; set; } = string.Empty;
        
        public string Role { get; set; } = string.Empty;
    }
}
