using CSharpFunctionalExtensions;
using TestRating.Application.Contacts.Test;

namespace TestRating.Application.Common.Interfaces
{
    public interface ITestExternalService
    {
        Task<bool> TestIsExists(long testId, CancellationToken cancellationToken = default);

        Task<Result<TestInfo>> GetTest(long testId, CancellationToken cancellationToken = default);
    }
}
