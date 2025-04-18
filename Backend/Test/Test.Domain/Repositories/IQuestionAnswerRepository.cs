using Test.Domain.Entities;

namespace Test.Domain.Repositories
{
    public interface IQuestionAnswerRepository
    {
        Task<QuestionAnswer> AddQuestionAnswer(
            QuestionAnswer questionAnswer, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<QuestionAnswer>> AddQuestionAnswers(
            List<QuestionAnswer> questionAnswers,
            CancellationToken cancellationToken = default);

        Task<QuestionAnswer?> GetQuestionAnswer(
            long id, 
            CancellationToken cancellationToken = default);
    }
}
