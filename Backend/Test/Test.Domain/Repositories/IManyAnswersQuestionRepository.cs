using Test.Domain.Entities;

namespace Test.Domain.Repositories
{
    public interface IManyAnswersQuestionRepository
    {
        Task<ManyAnswersQuestion> AddManyAnswerQuestion(ManyAnswersQuestion question, CancellationToken cancellationToken = default);

        Task<ManyAnswersQuestion?> GetManyAnswersQuestion(long id, CancellationToken cancellationToken = default);        
    }
}
