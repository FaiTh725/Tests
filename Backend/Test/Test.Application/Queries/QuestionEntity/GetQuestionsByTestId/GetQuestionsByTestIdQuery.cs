using MediatR;
using Test.Application.Contracts.Question;

namespace Test.Application.Queries.QuestionEntity.GetQuestionsByTestId
{
    public class GetQuestionsByTestIdQuery :
        IRequest<IEnumerable<QuestionWithAnswersResponse>>
    {
        public long TestId { get; set; }
    }
}
