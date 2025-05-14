namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface IOwnerAndAdminQuestionAccess
    {
        public long QuestionId { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }
    }
}
