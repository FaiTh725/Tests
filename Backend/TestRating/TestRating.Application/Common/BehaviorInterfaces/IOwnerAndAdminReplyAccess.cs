namespace TestRating.Application.Common.BehaviorInterfaces
{
    public interface IOwnerAndAdminReplyAccess
    {
        long ReplyId { get; set; }

        long ProfileId { get; set; }

        string ProfileRole { get; set; }
    }
}
