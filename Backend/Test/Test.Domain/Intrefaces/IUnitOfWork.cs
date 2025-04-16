using Test.Domain.Repositories;

namespace Test.Domain.Intrefaces
{
    public interface IUnitOfWork : INoSQLUnitOfWork
    {
        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        bool CanConnect();

        Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
    }
}
