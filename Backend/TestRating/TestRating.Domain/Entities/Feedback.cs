using CSharpFunctionalExtensions;
using TestRating.Domain.Validators;

namespace TestRating.Domain.Entities
{
    public class Feedback : Entity
    {
        public string ImageFolder { get => $"FeedBack-{Id}"; }

        public string Text { get; private set; }

        public Profile Owner { get; private set; }
        public long OwnerId { get; private set; }

        public int Rating { get; private set; }

        public DateTime SendTime { get; private set; }

        public DateTime UpdateTime { get; private set; }

        public Feedback() { }

        private Feedback(
            string text,
            int rating,
            Profile owner)
        {
            Text = text;
            Rating = rating;
            Owner = owner;

            SendTime = DateTime.UtcNow;
            UpdateTime = DateTime.UtcNow;
        }

        private Feedback(
            string text,
            int rating,
            long ownerId)
        {
            Text = text;
            Rating = rating;
            OwnerId = ownerId;

            SendTime = DateTime.UtcNow;
            UpdateTime = DateTime.UtcNow;
        }

        public static Result<Feedback> Initialize(
            string text,
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
                rating,
                owner));
        }

        public static Result<Feedback> Initialize(
            string text,
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
                rating,
                ownerId));
        }

        public static Result Validate(
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
