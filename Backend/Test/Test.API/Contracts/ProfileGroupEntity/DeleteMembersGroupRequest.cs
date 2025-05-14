namespace Test.API.Contracts.ProfileGroupEntity
{
    public class DeleteMembersGroupRequest
    {
        public long GroupId { get; set; }

        public List<long> MembersId { get; set; } = new List<long>();
    }
}
