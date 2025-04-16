using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class ManyAnswersQuestionRepository : IManyAnswersQuestionRepository
    {
        public Task<ManyAnswersQuestion> AddManyAnswerQuestion(ManyAnswersQuestion question, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ManyAnswersQuestion?> GetManyAnswersQuestion(long id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
