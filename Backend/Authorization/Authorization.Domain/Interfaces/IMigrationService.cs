namespace Authorization.Domain.Interfaces
{
    public interface IMigrationService
    {
        Task ApplyPendingMigrations();

        Task<IEnumerable<string>> GetPendingMigrations();
    }
}
