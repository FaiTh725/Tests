using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Infastructure.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Authorization.Infastructure
{
    public static class Startup
    {
        public static IServiceCollection ConfigureInfastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddCacheProvider(configuration);

            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IJwtService<UserTokenRequest, UserTokenResponse>, JwtUserService>();

            return services;
        }

        private static IServiceCollection AddCacheProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisCacheConncetion = configuration
                .GetConnectionString("RedisCacheConnection") ??
                throw new AppConfigurationException("Redis Cache conncetion string");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisCacheConncetion;
                options.InstanceName = "TestingAuth";
            });

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisCacheConncetion));

            return services;
        }
    }
}
