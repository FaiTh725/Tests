namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface IOwnerAndAdminQuestionAccess
    {
        public long QuestionId { get; set; }

        public long OwnerId { get; set; }

        public string Role { get; set; }
    }
}
