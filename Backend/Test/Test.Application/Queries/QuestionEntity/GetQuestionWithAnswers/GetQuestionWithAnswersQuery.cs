using MediatR;
using Test.Application.Contracts.Question;

namespace Test.Application.Queries.QuestionEntity.GetQuestionWithAnswers
{
    public class GetQuestionWithAnswersQuery:
        IRequest<QuestionWithAnswersResponse>
    {
        public long Id { get; set; }
    }
}
