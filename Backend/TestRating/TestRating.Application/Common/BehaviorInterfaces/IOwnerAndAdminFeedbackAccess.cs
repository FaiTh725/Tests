namespace TestRating.Application.Common.BehaviorInterfaces
{
    public interface IOwnerAndAdminFeedbackAccess
    {
        long FeedbackId { get; set; }

        long ProfileId { get; set; }

        string ProfileRole { get; set; }
    }
}
