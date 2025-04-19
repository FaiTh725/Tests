using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;
using Test.Application.Contracts.File;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Domain.Enums;

namespace Test.Application.Commands.Question.CreateQuestion
{
    public class CreateQuestionCommand : 
        IRequest<long>,
        IOwnerAndAdminTestAccess
    {
        public List<FileModel> QuestionImages { get; set; } = new List<FileModel>();

        public string TestQuestion { get; set; } = string.Empty;

        public int QuestionWeight { get; set; }

        public QuestionType QuestionType { get; set; }

        public long TestId { get; set; }

        public List<CreateQuestionAnswer> Answers { get; set; } = new List<CreateQuestionAnswer>();

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
