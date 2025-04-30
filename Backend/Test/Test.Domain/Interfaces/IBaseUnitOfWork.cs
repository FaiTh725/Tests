namespace Test.Domain.Interfaces
{
    public interface IBaseUnitOfWork: IDisposable
    {
        void BeginTransaction();

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        void CommitTransaction();

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        void RollBackTransaction();

        Task RollBackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
