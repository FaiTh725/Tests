using Test.Domain.Primitives;

namespace Test.Domain.Events
{
    public class MembersDeletedEvent : IDomainEvent
    {
        public long GroupId { get; set; }

        public List<long> MembersId { get; set; } = new List<long>();
    }
}
