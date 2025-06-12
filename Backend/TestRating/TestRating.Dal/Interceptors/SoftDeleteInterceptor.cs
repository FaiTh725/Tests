using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TestRating.Domain.Primitives;

namespace TestRating.Dal.Interceptors
{
    // TODO: delete it is useless
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, 
            InterceptionResult<int> result, 
            CancellationToken cancellationToken = default)
        {
            if(eventData.Context is null)
            {
                return base.SavingChangesAsync(
                eventData, result, cancellationToken);
            }

            IEnumerable<EntityEntry<ISoftDeletable>> entries = eventData
                .Context
                .ChangeTracker
                .Entries<ISoftDeletable>()
                .Where(x => x.State == EntityState.Deleted);

            foreach(EntityEntry<ISoftDeletable> softDeletable in entries)
            {
                softDeletable.State = EntityState.Modified;
                var deleteResult = softDeletable.Entity.Delete();
                
                var entity = softDeletable.Entity;

                if(deleteResult.IsFailure)
                {
                    throw new InvalidOperationException("Deleting deleted entity");
                }
            }

            return base.SavingChangesAsync(
                eventData, result, cancellationToken);
        }
    }
}
