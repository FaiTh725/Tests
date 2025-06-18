using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Application.Queries.ProfileAnswers.Specifications
{
    public class GetProfileQuestionAnswerForSessionSpecification : 
        BaseSpecification<ProfileAnswer>
    {
        public GetProfileQuestionAnswerForSessionSpecification(
            long sessionId)
        {
            Criteria = session => session.SessionId == sessionId;
        }
    }
}
