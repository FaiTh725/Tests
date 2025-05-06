namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface IOwnerAndAdminGroupAccess
    {
        public long OwnerId { get; set; }

        public long GroupId { get; set; }

        public string Role {  get; set; }
    }
}
