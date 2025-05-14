using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.QuestionAnswerEntity.Specifications
{
    public class AnswersByQuestionsIdSpecification : 
        BaseSpecification<QuestionAnswer>
    {
        public AnswersByQuestionsIdSpecification(
            List<long> questionId)
        {
            Criteria = questionAnswer => questionId.Contains(questionAnswer.QuestionId);
        }
    }
}
