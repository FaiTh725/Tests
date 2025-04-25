using CSharpFunctionalExtensions;
using Test.Application.Contracts.ProfileAnswerEntity;
using Test.Application.Contracts.Test;

namespace Test.Application.Common.Interfaces
{
    public interface ITestEvaluatorService
    {
        Task<Result<TestResult>> Evaluate(
            Dictionary<long, SessionProfileAnswer> profileAnswers,
            long testId,
            long testSessionId,
            CancellationToken cancellationToken = default);
    }
}
