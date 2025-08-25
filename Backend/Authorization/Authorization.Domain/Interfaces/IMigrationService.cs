namespace Authorization.Domain.Interfaces
{
    public interface IMigrationService
    {
        void ApplyPendingMigrations(CancellationToken cancellationToken = default);

        IEnumerable<string> GetPendingMigrations(CancellationToken cancellationToken = default);
    }
}
