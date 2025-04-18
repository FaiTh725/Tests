using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question> AddQuestion(Question question, CancellationToken cancellation = default);

        Task<Question?> GetQuestion(long id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Question>> GetQuestionsByCriteria(BaseSpecification<Question> specification, CancellationToken cancellationToken = default);
    }
}
