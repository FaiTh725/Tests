namespace Test.Application.Common.BehaviorsIntrfaces
{
    public interface IOwnerAndAdminTestAccess
    {
        public long TestId { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }
    }
}
