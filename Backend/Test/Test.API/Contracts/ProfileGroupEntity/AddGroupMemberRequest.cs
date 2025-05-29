namespace Test.API.Contracts.ProfileGroupEntity
{
    public class AddGroupMemberRequest
    {
        public long GroupId { get; set; }

        public long MemberId { get; set; }
    }
}
