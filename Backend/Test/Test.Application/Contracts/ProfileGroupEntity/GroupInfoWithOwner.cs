using Test.Application.Contracts.ProfileEntity;

namespace Test.Application.Contracts.ProfileGroupEntity
{
    public class GroupInfoWithOwner
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public required ProfileResponse Owner { get; set; }
    }
}
