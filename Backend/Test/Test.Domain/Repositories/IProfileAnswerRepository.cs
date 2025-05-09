using Test.Domain.Entities;
using Test.Domain.Primitives;

namespace Test.Domain.Repositories
{
    public interface IProfileAnswerRepository
    {
        Task<IEnumerable<ProfileAnswer>> AddProfileAnswers(List<ProfileAnswer> profileAnswers, IDatabaseSession? session = null,CancellationToken cancellationToken = default);
    }
}
