using Authorization.Application.Common.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Authorization.Infastructure.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache cache;

        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public CacheService(
            IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<Result<T>> GetData<T>(
            string key, CancellationToken cancellationToken = default)
        {
            var jsonData = await cache.GetStringAsync(key, cancellationToken);

            if(jsonData is null)
            {
                return Result.Failure<T>("Data doesnt exist");
            }

            var data = JsonSerializer.Deserialize<T>(jsonData, serializerOptions);

            if(data is null)
            {
                return Result.Failure<T>("Deserialize error");
            }

            return Result.Success(data);
        }

        public async Task RemoveData(string key, CancellationToken cancellationToken = default)
        {
            await cache.RemoveAsync(key, cancellationToken);
        }

        public async Task SetData<T>(
            string key, T data, int expirationTime, CancellationToken cancellationToken = default)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expirationTime)
            };

            var jsonData = JsonSerializer.Serialize(data, serializerOptions);

            await cache.SetStringAsync(key, jsonData, cacheOptions, cancellationToken);
        }
    }
}
