namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface IOwnerAndAdminQuestionAccess
    {
        long QuestionId { get; set; }

        long OwnerId { get; set; }

        string Role { get; set; }
    }
}
