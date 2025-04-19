using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.QuestionEntity.Specifications
{
    public class QuestionsByTestIdSpecification : 
        BaseSpecification<Question>
    {
        public QuestionsByTestIdSpecification(
            long tesId)
        {
            Criteria = question => question.TestId == tesId;
        }
    }
}
