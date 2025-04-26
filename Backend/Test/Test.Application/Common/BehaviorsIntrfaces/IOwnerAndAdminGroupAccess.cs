namespace Test.Application.Common.BehaviorsIntrfaces
{
    public interface IOwnerAndAdminGroupAccess
    {
        public long OwnerId { get; set; }

        public long GroupId { get; set; }

        public string Role {  get; set; }
    }
}
