using CSharpFunctionalExtensions;

namespace TestRating.Domain.Primitives
{
    public class DomainEventEntity : Entity, IDomainEventEntity
    {
        protected readonly List<IDomainEvent> events = new();

        public IReadOnlyList<IDomainEvent> DomainEvents => events.ToList();

        public void ClearDomainEvents()
        {
            events.Clear();
        }

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            events.Add(domainEvent);
        }
    }
}
