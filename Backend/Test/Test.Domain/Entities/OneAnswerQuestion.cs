using CSharpFunctionalExtensions;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public sealed class OneAnswerQuestion : QuestionBase
    {
        public long QuestionAnswerId { get; private set; }

        private OneAnswerQuestion(
            string testQuestion, 
            int questionWeight,
            long testId,
            long questionAnswerId) : 
            base(testQuestion, questionWeight, testId)
        {
            QuestionAnswerId = questionAnswerId;
        }

        public static Result<OneAnswerQuestion> Initialize(
            string testQuestion,
            int questionWeight,
            long testId,
            long questionAnswerId)
        {
            if(string.IsNullOrWhiteSpace(testQuestion) ||
                testQuestion.Length < QuestionValidator.MIN_QUESTION_LENGHT ||
                testQuestion.Length > QuestionValidator.MAX_QUESTION_LENGHT)
            {
                return Result.Failure<OneAnswerQuestion>("QUestion is null or white space " +
                    $"or lenght outside of {QuestionValidator.MIN_QUESTION_LENGHT} - {QuestionValidator.MAX_QUESTION_LENGHT}");
            }

            if(questionWeight < QuestionValidator.MIN_QUESTION_WEIGHT)
            {
                return Result.Failure<OneAnswerQuestion>($"Question weight less than {QuestionValidator.MIN_QUESTION_WEIGHT}");
            }

            return Result.Success(new OneAnswerQuestion(
                testQuestion,
                questionWeight,
                testId,
                questionAnswerId));
        }
    }
}
