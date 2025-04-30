using CSharpFunctionalExtensions;

namespace TestRating.Domain.Entities
{
    public class FeedbackReview : Entity
    {
        public bool IsPositive { get; private set; }

        public Profile Owner {  get; private set; }
        public long OwnerId { get; private set; }

        public FeedbackReview() {}

        private FeedbackReview(
            bool isPositive,
            Profile owner)
        {
            IsPositive = isPositive;
            Owner = owner;
        }

        private FeedbackReview(
            bool isPositive,
            long ownerId)
        {
            IsPositive = isPositive;
            OwnerId = ownerId;
        }

        public static Result<FeedbackReview> Initialize(
            bool isPositive,
            Profile owner)
        {
            if(owner is null)
            {
                return Result.Failure<FeedbackReview>("Owner is null");
            }

            return Result.Success(new FeedbackReview(
                isPositive,
                owner));
        }

        public static Result<FeedbackReview> Initialize(
            bool isPositive,
            long ownerId)
        {
            return Result.Success(new FeedbackReview(
                isPositive,
                ownerId));
        }
    }
}
