using MediatR;
using Test.Application.Common.BehaviorsInterfaces;

namespace Test.Application.Commands.Question.UpdateQuestion
{
    public class UpdateQuestionCommand : 
        IRequest<long>,
        IOwnerAndAdminQuestionAccess
    {    
        public int QuestionWeight { get; set; }

        public string TestQuestion { get; set; } = string.Empty;

        public long QuestionId { get; set; }

        public string Role { get; set; } = string.Empty;

        public long OwnerId { get; set; }
    }
}
