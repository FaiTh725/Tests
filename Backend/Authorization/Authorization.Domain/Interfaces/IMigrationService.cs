namespace Authorization.Domain.Interfaces
{
    public interface IMigrationService
    {
        void ApplyPendingMigrations();

        IEnumerable<string> GetPendingMigrations();
    }
}
