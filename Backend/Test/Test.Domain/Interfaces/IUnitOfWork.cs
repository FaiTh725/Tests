using Test.Domain.Repositories;

namespace Test.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IProfileRepository ProfileRepository { get; }

        ITestRepository TestRepository { get; }

        IQuestionRepository QuestionRepository { get; }

        IQuestionAnswerRepository QuestionAnswerRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        bool CanConnect();

        Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
    }
}
