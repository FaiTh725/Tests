using CSharpFunctionalExtensions;

namespace Authorization.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task SetData<T>(string key, T data, int expirationTime, CancellationToken cancellationToken = default);

        Task RemoveData(string key, CancellationToken cancellationToken = default);

        Task<Result<T>> GetData<T>(string key, CancellationToken cancellationToken = default);
    }
}
