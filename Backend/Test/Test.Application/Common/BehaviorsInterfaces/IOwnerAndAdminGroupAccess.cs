namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface IOwnerAndAdminGroupAccess
    {
        public string OwnerEmail { get; set; }

        public long GroupId { get; set; }

        public string Role {  get; set; }
    }
}
