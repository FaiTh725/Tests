using Test.Domain.Entities;

namespace Test.Domain.Repositories
{
    public interface IQuestionAnswerRepository
    {
        Task<QuestionAnswer> AddQuestionAnswer(
            QuestionAnswer questionAnswer, 
            CancellationToken cancellationToken = default);

        Task<QuestionAnswer> GetQuestionAnswer(
            long id, 
            CancellationToken cancellationToken = default);
    }
}
