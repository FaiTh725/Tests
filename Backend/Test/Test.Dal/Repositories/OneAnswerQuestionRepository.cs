using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class OneAnswerQuestionRepository : IOneAnswerQuestionRepository
    {
        public Task<OneAnswerQuestion> AddOneAnswerQuestion(OneAnswerQuestion question, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<OneAnswerQuestion?> GetOneAnswerQuestion(long id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
