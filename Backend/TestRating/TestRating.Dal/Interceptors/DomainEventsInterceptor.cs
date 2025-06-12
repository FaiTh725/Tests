using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TestRating.Domain.Primitives;

namespace TestRating.Dal.Interceptors
{
    public class DomainEventsInterceptor : SaveChangesInterceptor
    {
        private readonly IMediator mediator;

        public DomainEventsInterceptor(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, 
            int result, 
            CancellationToken cancellationToken = default)
        {
            var baseResult = await base.SavedChangesAsync(
                eventData, 
                result, 
                cancellationToken);

            if(eventData.Context is not null)
            {
                await PublishDomainEvents(eventData.Context.ChangeTracker);
            }

            return baseResult;
        }

        private async Task PublishDomainEvents(
            ChangeTracker changeTracker)
        {
            var domainEvents = changeTracker
                .Entries<IDomainEventEntity>()
                .Select(x => x.Entity)
                .SelectMany(entity =>
                {
                    var domainEvents = entity.DomainEvents;

                    entity.ClearDomainEvents();

                    return domainEvents;
                })
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
