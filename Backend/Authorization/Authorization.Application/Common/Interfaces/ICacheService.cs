using CSharpFunctionalExtensions;

namespace Authorization.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task SetData<T>(string key, T data, int expirationTime);

        Task RemoveData(string key);

        Task<Result<T>> GetData<T>(string key);
    }
}
