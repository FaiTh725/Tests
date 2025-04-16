using CSharpFunctionalExtensions;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public sealed class ManyAnswersQuestion : QuestionBase
    {
        public List<long> QuestionAnswerId { get; private set; }

        private ManyAnswersQuestion(
            string testQuestion, 
            int questionWeight,
            long testId) : 
            base(testQuestion, questionWeight, testId)
        {
            QuestionAnswerId = new List<long>();
        }

        private ManyAnswersQuestion(
            string testQuestion,
            int questionWeight,
            long testId,
            List<long> questionAnswerId):
            this(testQuestion, questionWeight, testId)
        {
            QuestionAnswerId = questionAnswerId;
        }

        public static Result<ManyAnswersQuestion> Initialize(
            string testQuestion,
            int questionWeight,
            long testId)
        {
            var isValid = Validate(
                testQuestion,
                questionWeight,
                testId);

            if(isValid.IsFailure)
            {
                return Result.Failure<ManyAnswersQuestion>(isValid.Error);
            }

            return Result.Success(new ManyAnswersQuestion(
                testQuestion,
                questionWeight, 
                testId));
        }

        public static Result<ManyAnswersQuestion> Initialize(
            string testQuestion,
            int questionWeight,
            long testId,
            List<long> questionAnswerId)
        {
            var isValid = Validate(
                testQuestion,
                questionWeight,
                testId);

            if (isValid.IsFailure)
            {
                return Result.Failure<ManyAnswersQuestion>(isValid.Error);
            }

            if(questionAnswerId is null)
            {
                return Result.Failure<ManyAnswersQuestion>("QuestionAnswerId list is null");
            }

            return Result.Success(new ManyAnswersQuestion(
                testQuestion,
                questionWeight,
                testId,
                questionAnswerId));
        }

        public static Result Validate(
            string testQuestion,
            int questionWeight,
            long testId)
        {
            if (string.IsNullOrWhiteSpace(testQuestion) ||
                testQuestion.Length < QuestionValidator.MIN_QUESTION_LENGHT ||
                testQuestion.Length > QuestionValidator.MAX_QUESTION_LENGHT)
            {
                return Result.Failure("QUestion is null or white space " +
                    $"or lenght outside of {QuestionValidator.MIN_QUESTION_LENGHT} - {QuestionValidator.MAX_QUESTION_LENGHT}");
            }

            if (questionWeight < QuestionValidator.MIN_QUESTION_WEIGHT)
            {
                return Result.Failure($"Question weight less than {QuestionValidator.MIN_QUESTION_WEIGHT}");
            }

            return Result.Success();
        }
    }
}
