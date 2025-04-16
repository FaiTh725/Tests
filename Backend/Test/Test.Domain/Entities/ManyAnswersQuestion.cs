using CSharpFunctionalExtensions;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public sealed class ManyAnswersQuestion : QuestionBase
    {
        public List<long> QuestionAnswerId { get; private set; }

        private ManyAnswersQuestion(
            string testQuestion, 
            int questionWeight) : 
            base(testQuestion, questionWeight)
        {
            QuestionAnswerId = new List<long>();
        }

        public static Result<ManyAnswersQuestion> Initialize(
            string testQuestion,
            int questionWeight)
        {
            if (string.IsNullOrWhiteSpace(testQuestion) ||
                testQuestion.Length < QuestionValidator.MIN_QUESTION_LENGHT ||
                testQuestion.Length > QuestionValidator.MAX_QUESTION_LENGHT)
            {
                return Result.Failure<ManyAnswersQuestion>("QUestion is null or white space " +
                    $"or lenght outside of {QuestionValidator.MIN_QUESTION_LENGHT} - {QuestionValidator.MAX_QUESTION_LENGHT}");
            }

            if (questionWeight < QuestionValidator.MIN_QUESTION_WEIGHT)
            {
                return Result.Failure<ManyAnswersQuestion>($"Question weight less than {QuestionValidator.MIN_QUESTION_WEIGHT}");
            }

            return Result.Success(new ManyAnswersQuestion(
                testQuestion,
                questionWeight));
        }
    }
}
