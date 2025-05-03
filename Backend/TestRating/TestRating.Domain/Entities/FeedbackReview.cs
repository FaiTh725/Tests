using CSharpFunctionalExtensions;

namespace TestRating.Domain.Entities
{
    public class FeedbackReview : Entity
    {
        public bool IsPositive { get; private set; }

        public Profile Owner {  get; private set; }
        public long OwnerId { get; private set; }

        public Feedback ReviewedFeedback { get; private set; }
        public long ReviewedFeedbackId { get; private set; }

        public FeedbackReview() {}

        private FeedbackReview(
            bool isPositive,
            Profile owner,
            Feedback reviewedFeedback)
        {
            IsPositive = isPositive;
            Owner = owner;
            ReviewedFeedback = reviewedFeedback;
        }

        private FeedbackReview(
            bool isPositive,
            long ownerId,
            long reviewedFeedbackId)
        {
            IsPositive = isPositive;
            OwnerId = ownerId;
            ReviewedFeedbackId = reviewedFeedbackId;
        }

        public void ChangeReview(bool isPositive)
        {
            IsPositive = isPositive;
        }


        public static Result<FeedbackReview> Initialize(
            bool isPositive,
            Profile owner,
            Feedback reviewedFeedback)
        {
            if(owner is null)
            {
                return Result.Failure<FeedbackReview>("Owner is null");
            }

            return Result.Success(new FeedbackReview(
                isPositive,
                owner,
                reviewedFeedback));
        }

        public static Result<FeedbackReview> Initialize(
            bool isPositive,
            long ownerId,
            long reviewedFeedbackId)
        {
            return Result.Success(new FeedbackReview(
                isPositive,
                ownerId,
                reviewedFeedbackId));
        }
    }
}
