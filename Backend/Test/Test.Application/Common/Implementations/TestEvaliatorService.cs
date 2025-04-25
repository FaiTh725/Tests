using CSharpFunctionalExtensions;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileAnswerEntity;
using Test.Application.Contracts.Test;
using Test.Domain.Entities;
using Test.Domain.Intrefaces;

namespace Test.Application.Common.Implementations
{
    public class TestEvaliatorService : ITestEvaluatorService
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public TestEvaliatorService(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<TestResult>> Evaluate(
            Dictionary<long, SessionProfileAnswer> profileAnswers, 
            long testId, 
            long testSessionId,
            CancellationToken cancellationToken = default)
        {
            var correctAnswers = await unitOfWork.QuestionRepository
                    .GetQuestionCorrectAnswers(testId, cancellationToken);

            var profileAnswersEntity = new List<ProfileAnswer>();
            var profilePoints = 0;
            var maxPoints = 0;

            foreach (var correctAnswer in correctAnswers)
            {
                var question = await unitOfWork.QuestionRepository
                    .GetQuestion(correctAnswer.Key, cancellationToken);

                if (question is null)
                {
                    return Result.Failure<TestResult>("Question doesnt exist");
                }

                profileAnswers.TryGetValue(correctAnswer.Key, out SessionProfileAnswer? profileAnswer);

                var correctAnswersId = correctAnswer.Value.Select(x => x.Id);

                var isCorrectAnswer = profileAnswer is null ||
                    !correctAnswersId.ToHashSet().SetEquals(profileAnswer.QuestionAnswersId);

                profilePoints += isCorrectAnswer ? question.QuestionWeight: 0;
                maxPoints += question.QuestionWeight;

                var profileAnswerEntity = ProfileAnswer.Initialize(
                    testSessionId,
                    correctAnswer.Key,
                    profileAnswer is null ?
                        new List<long>() :
                        profileAnswer.QuestionAnswersId,
                    isCorrectAnswer);

                if (profileAnswerEntity.IsFailure)
                {
                    return Result.Failure<TestResult>("Error initialize entity from session data");
                }

                profileAnswersEntity.Add(profileAnswerEntity.Value);
            }

            return Result.Success(new TestResult
            {
                TestId = testId,
                ProfileAnswers = profileAnswersEntity,
                Percent = profilePoints * 100 / maxPoints
            });
        }
    }
}
