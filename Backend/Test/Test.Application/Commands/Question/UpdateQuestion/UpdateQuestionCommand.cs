using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;

namespace Test.Application.Commands.Question.UpdateQuestion
{
    public class UpdateQuestionCommand : 
        IRequest<long>,
        IOwnerAndAdminQuestionAccess
    {    
        public int QuestionWeight { get; set; }

        public string TestQuestion { get; set; } = string.Empty;

        public long QuestionId { get; set; }
        
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
