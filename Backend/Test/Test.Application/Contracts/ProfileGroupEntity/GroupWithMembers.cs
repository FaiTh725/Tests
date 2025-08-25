using Test.Application.Contracts.ProfileEntity;

namespace Test.Application.Contracts.ProfileGroupEntity
{
    public class GroupWithMembers
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public IEnumerable<ProfileResponse> Members { get; set; } = new List<ProfileResponse>();
    }
}
