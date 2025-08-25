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

        public void ApplyPendingMigrations()
        {
            context.Database.Migrate();
        }

        public IEnumerable<string> GetPendingMigrations()
        {
            return context.Database.GetPendingMigrations();
        }
    }
}
