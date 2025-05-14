using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.QuestionAnswerEntity.Specifications
{
    public class AnswersByQuestionIdSpecification : 
        BaseSpecification<QuestionAnswer>
    {
        public AnswersByQuestionIdSpecification(
            long questionId)
        {
            Criteria = questionAnswer => questionAnswer.QuestionId == questionId;
        }
    }
}
