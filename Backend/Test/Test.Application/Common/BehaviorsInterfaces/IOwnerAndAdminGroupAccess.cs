namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface IOwnerAndAdminGroupAccess
    {
        long GroupId { get; set; }

        long OwnerId { get; set; }

        string Role {  get; set; }
    }
}
