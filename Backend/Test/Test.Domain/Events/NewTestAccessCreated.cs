using Test.Domain.Primitives;

namespace Test.Domain.Events
{
    public class NewTestAccessCreated : IDomainEvent
    {
        public long TestAccessId { get; set; }
    }
}
