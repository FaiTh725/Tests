namespace Authorization.Domain.Interfaces
{
    public interface IMigrationService
    {
        Task ApplyPendingMigrations(CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetPendingMigrations(CancellationToken cancellationToken = default);
    }
}
