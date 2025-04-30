namespace TestRating.Domain.Validators
{
    public static class FeedbackValidator
    {
        public const int MIN_FEEDBACK_MESSAGE_LENGTH = 2;

        public const int MAX_FEEDBACK_MESSAGE_LENGTH = 500;

        public const int MIN_FEEDBACK_RATING = 0;

        public const int MAX_FEEDBACK_RATING = 10;
    }
}
