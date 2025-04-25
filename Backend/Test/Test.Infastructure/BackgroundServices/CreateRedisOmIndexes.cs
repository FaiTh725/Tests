using Microsoft.Extensions.Hosting;
using Redis.OM;
using Test.Infastructure.RedisEntities;

namespace Test.Infastructure.BackgroundServices
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
