using CSharpFunctionalExtensions;
using Test.Domain.Enums;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class Question : Entity
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

        public static Result<Question> Initialize(
            string testQuestion,
            int questionWeight,
            QuestionType questionType,
            long testId)
        {
            if (string.IsNullOrWhiteSpace(testQuestion) ||
                testQuestion.Length < QuestionValidator.MIN_QUESTION_LENGHT ||
                testQuestion.Length > QuestionValidator.MAX_QUESTION_LENGHT)
            {
                return Result.Failure<Question>("QUestion is null or white space " +
                    $"or lenght outside of {QuestionValidator.MIN_QUESTION_LENGHT} - {QuestionValidator.MAX_QUESTION_LENGHT}");
            }

            if (questionWeight < QuestionValidator.MIN_QUESTION_WEIGHT)
            {
                return Result.Failure<Question>($"Question weight less than {QuestionValidator.MIN_QUESTION_WEIGHT}");
            }

            return Result.Success(new Question(
                testQuestion,
                questionWeight,
                questionType,
                testId));
        }
    }
}
