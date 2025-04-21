using Test.Domain.Entities;

namespace Test.Domain.Repositories
{
    public interface IProfileAnswerRepository
    {
        Task<IEnumerable<ProfileAnswer>> AddProfileAnswers(List<ProfileAnswer> profileAnswers, CancellationToken cancellationToken = default);
    }
}
