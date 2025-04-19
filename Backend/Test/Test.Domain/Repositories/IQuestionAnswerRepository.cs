using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface IQuestionAnswerRepository
    {
        Task<QuestionAnswer> AddQuestionAnswer(QuestionAnswer questionAnswer, CancellationToken cancellationToken = default);

        Task<IEnumerable<QuestionAnswer>> AddQuestionAnswers(List<QuestionAnswer> questionAnswers, CancellationToken cancellationToken = default);

        Task<QuestionAnswer?> GetQuestionAnswer(long id, CancellationToken cancellationToken = default);

        Task<IEnumerable<QuestionAnswer>> GetQuestionAnswersByCriteria(BaseSpecification<QuestionAnswer> specification, CancellationToken cancellationToken = default);

        Task DeleteManyByCriteria(BaseSpecification<QuestionAnswer> specification, CancellationToken cancellationToken = default);

        Task DeleteAnswers(List<long> idList, CancellationToken cancellationToken = default);
    }
}
