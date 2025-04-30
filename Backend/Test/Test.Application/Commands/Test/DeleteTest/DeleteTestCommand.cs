using MediatR;
using Test.Application.Common.BehaviorsInterfaces;

namespace Test.Application.Commands.Test.DeleteTest
{
    public class DeleteTestCommand : 
        IRequest,
        IOwnerAndAdminTestAccess
    {
        public long TestId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
