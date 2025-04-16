﻿namespace Test.Domain.Primitives
{
    public interface IDomainEventEntity
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }

        void ClearDomainEvents();
    }
}
