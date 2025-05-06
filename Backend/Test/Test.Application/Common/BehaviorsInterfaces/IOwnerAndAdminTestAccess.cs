namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface IOwnerAndAdminTestAccess
    {
        public long TestId { get; set; }

        public long OwnerId { get; set; }

        public string Role { get; set; }
    }
}
