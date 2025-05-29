using Test.Domain.Enums;

namespace Test.API.Contracts.TestAccess
{
    public class ProvideTestAccessRequest
    {
        public long TestId { get; set; }

        public long TargetEntityId { get; set; }

        public TargetAccessEntityType TargetAccessEntityType { get; set; }
    }
}
