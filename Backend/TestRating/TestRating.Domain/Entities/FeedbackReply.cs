using CSharpFunctionalExtensions;
using TestRating.Domain.Primitives;
using TestRating.Domain.Validators;

namespace TestRating.Domain.Entities
{
    public class FeedbackReply : DomainEventEntity, ISoftDeletable
    {
        public string Text { get; private set; }

        public Feedback Feedback { get; private set; }
        public long FeedbackId { get; private set; }

        public Profile Owner { get; private set; }
        public long OwnerId { get; private set; }

        public DateTime SendTime { get; private set; }

        public DateTime UpdateTime { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime? DeletedTime { get; private set; }

        public FeedbackReply(){}

        private FeedbackReply(
            string text,
            long feedbackId,
            long ownerId)
        {
            Text = text;
            FeedbackId = feedbackId;
            OwnerId = ownerId;

            SendTime = DateTime.UtcNow;
            UpdateTime = DateTime.UtcNow;
            IsDeleted = false;
        }

        public Result Delete()
        {
            if (IsDeleted)
            {
                return Result.Failure("Reply has already deleted");
            }

            IsDeleted = true;
            DeletedTime = DateTime.UtcNow;

            return Result.Success();
        }

        public Result ChangeReply(
            string text)
        {
            var isValid = Validate(text);

            if (isValid.IsFailure)
            {
                return Result.Failure(isValid.Error);
            }

            Text = text;
            UpdateTime = DateTime.UtcNow;

            return Result.Success();
        }

        public static Result<FeedbackReply> Initialize(
            string text,
            long feedbackId,
            long ownerId)
        {
            var isValid = Validate(text);

            if(isValid.IsFailure)
            {
                return Result.Failure<FeedbackReply>(isValid.Error);
            }

            return Result.Success(new FeedbackReply(
                text,
                feedbackId,
                ownerId));
        }

        private static Result Validate(
            string text)
        {
            if (string.IsNullOrWhiteSpace(text) ||
                text.Length < FeedbackReplyValidator.MIN_REPLY_MESSAGE_LENGTH ||
                text.Length > FeedbackReplyValidator.MAX_REPLY_MESSAGE_LENGTH)
            {
                return Result.Failure("Text is null or white space or outside of " +
                    $"{FeedbackReplyValidator.MIN_REPLY_MESSAGE_LENGTH} {FeedbackReplyValidator.MAX_REPLY_MESSAGE_LENGTH}");
            }

            return Result.Success();
        }
    }
}
