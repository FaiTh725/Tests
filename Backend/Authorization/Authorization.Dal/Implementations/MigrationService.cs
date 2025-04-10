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

        public async Task ApplyPendingMigrations()
        {
            await context.Database.MigrateAsync();
        }

        public async Task<IEnumerable<string>> GetPendingMigrations()
        {
            return await context.Database.GetPendingMigrationsAsync();
        }
    }
}
