using Test.Domain.Repositories;

namespace Test.Domain.Intrefaces
{
    public interface INoSQLUnitOfWork: IDisposable
    {
        public IProfileRepository ProfileRepository { get; }

        public ITestRepository TestRepository { get; }

        public IQuestionRepository QuestionRepository { get; }

        public IQuestionAnswerRepository QuestionAnswerRepository { get; }

        void BeginTransaction();

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        void CommitTransaction();

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        void RollBackTransaction();

        Task RollBackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
