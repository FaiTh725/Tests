using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;
using Test.Application.Common.Interfaces;

namespace Test.Infrastructure.Implementations
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache cache;

        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public RedisCacheService(
            IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<Result<T>> GetData<T>(
            string key, 
            CancellationToken cancellationToken = default)
        {
            var jsonData = await cache.GetStringAsync(key, cancellationToken);

            if (jsonData == null)
            {
                return Result.Failure<T>("Data doesnt set");
            }

            var data = JsonSerializer.Deserialize<T>(jsonData, serializerOptions);

            if(data is null)
            {
                return Result.Failure<T>("Error with deserialize data");
            }

            return Result.Success(data);
        }

        public async Task RemoveData(
            string key, 
            CancellationToken cancellationToken = default)
        {
            await cache.RemoveAsync(key, cancellationToken);
        }

        public async Task SetData<T>(
            string key, 
            T data, 
            TimeSpan expirationTime, 
            CancellationToken cancellationToken = default)
        {
            var jsonData = JsonSerializer.Serialize(data, serializerOptions);

            var cacheOptions = new DistributedCacheEntryOptions
            { 
                AbsoluteExpirationRelativeToNow = expirationTime
            };

            await cache.SetStringAsync(key, jsonData, cacheOptions, cancellationToken);
        }

        public async Task SetData<T>(
            string key, 
            T data, 
            CancellationToken cancellationToken = default)
        {
            var jsonData = JsonSerializer.Serialize(data, serializerOptions);

            await cache.SetStringAsync(key, jsonData, cancellationToken);
        }
    }
}
