using Test.Domain.Primitives;

namespace Test.Domain.Events
{
    public class AddedNewGroupMember : IDomainEvent
    {
        public long ProfileId { get; set; }

        public long GroupId { get; set; }
    }
}
