using CSharpFunctionalExtensions;
using Test.Domain.Enums;
using Test.Domain.Events;
using Test.Domain.Primitives;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class Question : DomainEventEntity
    {
        public string ImageFolder { get => $"Question-{Id}"; }

        public string TestQuestion {  get; private set; }

        public int QuestionWeight { get; private set; }

        public QuestionType QuestionType { get; private set; }

        public long TestId { get; private set; }

        private Question(
            string testQuestion,
            int questionWeight,
            QuestionType questionType,
            long testId)
        {
            TestQuestion = testQuestion;
            QuestionWeight = questionWeight;
            QuestionType = questionType;
            TestId = testId;
        }

        public void Delete()
        {
            RaiseDomainEvent(new QuestionDeletedEvent(Id));
        }

        public Result Update(
            string testQuestion,
            int questionWeight)
        {
            var isValid = Validate(
                testQuestion,
                questionWeight);

            if(isValid.IsFailure)
            {
                return Result.Failure(isValid.Error);
            }

            TestQuestion = testQuestion;
            QuestionWeight = questionWeight;

            return Result.Success();
        }

        public static Result<Question> Initialize(
            string testQuestion,
            int questionWeight,
            QuestionType questionType,
            long testId)
        {
            var isValid = Validate(
                testQuestion,
                questionWeight);

            if(isValid.IsFailure)
            {
                return Result.Failure<Question>(isValid.Error);
            }

            return Result.Success(new Question(
                testQuestion,
                questionWeight,
                questionType,
                testId));
        }

        private static Result Validate(
            string testQuestion,
            int questionWeight)
        {
            if (string.IsNullOrWhiteSpace(testQuestion) ||
                testQuestion.Length < QuestionValidator.MIN_QUESTION_LENGTH ||
                testQuestion.Length > QuestionValidator.MAX_QUESTION_LENGTH)
            {
                return Result.Failure("QUestion is null or white space " +
                    $"or length outside of {QuestionValidator.MIN_QUESTION_LENGTH} - {QuestionValidator.MAX_QUESTION_LENGTH}");
            }

            if (questionWeight < QuestionValidator.MIN_QUESTION_WEIGHT)
            {
                return Result.Failure($"Question weight less than {QuestionValidator.MIN_QUESTION_WEIGHT}");
            }

            return Result.Success();
        }
    }
}
