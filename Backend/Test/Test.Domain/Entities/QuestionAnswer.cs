using CSharpFunctionalExtensions;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class QuestionAnswer : Entity
    {
        public string ImageFolder { get => $"Answer-{Id}"; }

        public string Answer {  get; private set; }

        public bool IsCorrect {  get; private set; }

        public long QuestionId { get; private set; }

        private QuestionAnswer(
            string answer,
            bool isCorrect,
            long questionId) 
        { 
            Answer = answer;
            IsCorrect = isCorrect;
            QuestionId = questionId;
        }

        public static Result<QuestionAnswer> Initialize(
            string answer,
            bool isCorrect,
            long questionId)
        {
            if(string.IsNullOrWhiteSpace(answer) ||
                answer.Length < QuestionAnswerValidator.MIN_ANSWER_LENGTH ||
                answer.Length > QuestionAnswerValidator.MAX_ANSWER_LENGTH)
            {
                return Result.Failure<QuestionAnswer>("Answer is null or white space " +
                    $"or length outside from range {QuestionAnswerValidator.MIN_ANSWER_LENGTH} - {QuestionAnswerValidator.MAX_ANSWER_LENGTH}");
            }

            return Result.Success(new QuestionAnswer(
                answer, isCorrect, questionId));
        }
    }
}
