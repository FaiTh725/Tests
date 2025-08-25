using CSharpFunctionalExtensions;

namespace Test.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task SetData<T>(string key, T data, TimeSpan expirationTime, CancellationToken cancellationToken = default);

        Task SetData<T>(string key, T data, CancellationToken cancellationToken = default);

        Task RemoveData(string key, CancellationToken cancellationToken = default);

        Task<Result<T>> GetData<T>(string key, CancellationToken cancellationToken = default);
    }
}
