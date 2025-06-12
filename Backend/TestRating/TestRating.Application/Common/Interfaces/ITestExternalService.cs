namespace TestRating.Application.Common.Interfaces
{
    public interface ITestExternalService
    {
        Task<bool> TestIsExists(long testId, CancellationToken cancellationToken = default);
    }
}
