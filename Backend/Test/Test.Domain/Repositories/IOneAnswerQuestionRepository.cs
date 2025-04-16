using Test.Domain.Entities;

namespace Test.Domain.Repositories
{
    public interface IOneAnswerQuestionRepository
    {
        Task<OneAnswerQuestion> AddOneAnswerQuestion(OneAnswerQuestion question, CancellationToken cancellationToken = default);

        Task<OneAnswerQuestion?> GetOneAnswerQuestion(long id, CancellationToken cancellationToken = default);
    }
}
