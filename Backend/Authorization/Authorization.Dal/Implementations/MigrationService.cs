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

        public void ApplyPendingMigrations(
            CancellationToken cancellationToken = default)
        {
            context.Database.Migrate();
        }

        public IEnumerable<string> GetPendingMigrations(
            CancellationToken cancellationToken = default)
        {
            return context.Database.GetPendingMigrations();
        }
    }
}
