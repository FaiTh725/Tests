namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface IOwnerAndAdminTestAccess
    {
        long TestId { get; set; }

        long OwnerId { get; set; }

        string Role { get; set; }
    }
}
