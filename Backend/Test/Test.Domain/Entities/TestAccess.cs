using CSharpFunctionalExtensions;

namespace Test.Domain.Entities
{
    public class TestAccess : Entity
    {
        public long TestId { get; set; }

        public long AvailableId { get; set; }
    }
}
