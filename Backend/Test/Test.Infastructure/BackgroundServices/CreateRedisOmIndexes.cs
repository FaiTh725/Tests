using Microsoft.Extensions.Hosting;
using Redis.OM;
using Test.Infrastructure.RedisEntities;

namespace Test.Infrastructure.BackgroundServices
{
    public class CreateRedisOmIndexes : BackgroundService
    {
        private readonly RedisConnectionProvider redisProvider;

        public CreateRedisOmIndexes(
            RedisConnectionProvider redisProvider)
        {
            this.redisProvider = redisProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await redisProvider.Connection.CreateIndexAsync(typeof(RedisTestSession));
        }
    }
}
