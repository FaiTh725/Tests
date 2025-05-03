using CSharpFunctionalExtensions;
using TestRating.Domain.Events;
using TestRating.Domain.Primitives;
using TestRating.Domain.Validators;

namespace TestRating.Domain.Entities
{
    public class Feedback : DomainEventEntity, ISoftDeletable
    {
        public string ImageFolder { get => $"FeedBack-{Id}"; }

        public string Text { get; private set; }

        public long TestId { get; private set; }

        public Profile Owner { get; private set; }
        public long OwnerId { get; private set; }

        public List<FeedbackReview> Reviews { get; private set; }

        public int Rating { get; private set; }

        public DateTime SendTime { get; private set; }

        public DateTime UpdateTime { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime? DeletedTime { get; private set; }

        public Feedback() { }

        private Feedback(
            string text,
            long testId,
            int rating,
            Profile owner)
        {
            Text = text;
            TestId = testId;
            Rating = rating;
            Owner = owner;

            SendTime = DateTime.UtcNow;
            UpdateTime = DateTime.UtcNow;
            Reviews = new List<FeedbackReview>();
            IsDeleted = false;
        }

        private Feedback(
            string text,
            long testId,
            int rating,
            long ownerId)
        {
            Text = text;
            TestId = testId;
            Rating = rating;
            OwnerId = ownerId;

            SendTime = DateTime.UtcNow;
            UpdateTime = DateTime.UtcNow;
            Reviews = new List<FeedbackReview>();
            IsDeleted = false;
        }

        public Result Delete()
        {
            if (IsDeleted)
            {
                return Result.Failure("Feedback has already deleted");
            }

            IsDeleted = true;
            DeletedTime = DateTime.UtcNow;

            RaiseDomainEvent(new FeedbackDeletedEvent
            {
                FeedbackId = Id
            });

            return Result.Success();
        }

        public Result ChangeFeedback(
            string text,
            int rating)
        {
            var isValidUpdate = Validate(text, rating);

            if(isValidUpdate.IsFailure)
            {
                return Result.Failure("Invalid values to update feedback - " +
                    $"{isValidUpdate.Error}");
            }

            Text = text;
            Rating = rating;
            UpdateTime = DateTime.UtcNow;
        
            return Result.Success();
        }


        public static Result<Feedback> Initialize(
            string text,
            long testId,
            int rating,
            Profile owner)
        {
            var isValid = Validate(text, rating);

            if (isValid.IsFailure)
            {
                return Result.Failure<Feedback>(isValid.Error);
            }

            if(owner is null)
            {
                return Result.Failure<Feedback>("Owner is null");
            }

            return Result.Success(new Feedback(
                text,
                testId,
                rating,
                owner));
        }

        public static Result<Feedback> Initialize(
            string text,
            long testId,
            int rating,
            long ownerId)
        {
            var isValid = Validate(text, rating);

            if(isValid.IsFailure)
            {
                return Result.Failure<Feedback>(isValid.Error);
            }

            return Result.Success(new Feedback(
                text,
                testId,
                rating,
                ownerId));
        }

        private static Result Validate(
            string text,
            int rating)
        {
            if(string.IsNullOrWhiteSpace(text) ||
                text.Length < FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH ||
                text.Length > FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH)
            {
                return Result.Failure("Text is empty or white space or outside of " +
                    $"{FeedbackValidator.MIN_FEEDBACK_MESSAGE_LENGTH} - {FeedbackValidator.MAX_FEEDBACK_MESSAGE_LENGTH}");
            }

            if(rating < FeedbackValidator.MIN_FEEDBACK_RATING ||
                rating > FeedbackValidator.MAX_FEEDBACK_RATING)
            {
                return Result.Failure("Rating should be in range " +
                    $"from {FeedbackValidator.MIN_FEEDBACK_RATING} to {FeedbackValidator.MAX_FEEDBACK_RATING}");
            }

            return Result.Success();
        }
    }
}
