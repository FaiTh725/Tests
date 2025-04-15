using Authorization.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Dal.Implementations
{
    public class MigrationService : IMigrationService
    {
        private readonly AppDbContext context;

        public MigrationService(
            AppDbContext context)
        {
            this.context = context; 
        }

        public async Task ApplyPendingMigrations(
            CancellationToken cancellationToken = default)
        {
            await context.Database.MigrateAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetPendingMigrations(
            CancellationToken cancellationToken = default)
        {
            return await context.Database.GetPendingMigrationsAsync(cancellationToken);
        }
    }
}
