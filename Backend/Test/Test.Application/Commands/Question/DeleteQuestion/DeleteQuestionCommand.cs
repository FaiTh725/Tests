using MediatR;
using Test.Application.Common.BehaviorsInterfaces;

namespace Test.Application.Commands.Question.DeleteQuestion
{
    public class DeleteQuestionCommand : 
        IRequest,
        IOwnerAndAdminQuestionAccess
    {
        public long QuestionId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
