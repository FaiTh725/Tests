using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question> AddQuestion(Question question, IDatabaseSession? session = null, CancellationToken cancellation = default);

        Task<Question?> GetQuestion(long id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Question>> GetQuestionsByCriteria(BaseSpecification<Question> specification, CancellationToken cancellationToken = default);

        Task DeleteQuestions(List<long> questionIdList, IDatabaseSession? session = null, CancellationToken cancellationToken = default);

        Task DeleteQuestion(long id, IDatabaseSession? session = null, CancellationToken cancellationToken = default);

        Task UpdateQuestion(long id, Question updatedQuestion, IDatabaseSession? session = null, CancellationToken cancellationToken = default);

        Task<Dictionary<long, IEnumerable<QuestionAnswer>>> GetQuestionCorrectAnswers(long testId, CancellationToken cancellationToken = default);
    }
}
