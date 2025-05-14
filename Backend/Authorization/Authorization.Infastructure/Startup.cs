using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Infastructure.Configurations;
using MassTransit;
using Authorization.Infrastructure.BackgroundServices;
using Authorization.Infrastructure.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Authorization.Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection ConfigureInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddCacheProvider(configuration)
                .AddMasstransitProvider(configuration);

            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IJwtService<UserTokenRequest, UserTokenResponse>, JwtUserService>();

            services.AddHostedService<ApplyMigrationsBackgroundService>();
            services.AddHostedService<InitializeRolesBackgroundService>();

            return services;
        }

        private static IServiceCollection AddCacheProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisCacheConncetion = configuration
                .GetConnectionString("RedisCacheConnection") ??
                throw new AppConfigurationException("Redis Cache connection string");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisCacheConncetion;
                options.InstanceName = "TestingAuth";
            });

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisCacheConncetion));

            return services;
        }

        private static IServiceCollection AddMasstransitProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var rabbitMqConf = configuration
                .GetSection("RabbitMqSettings")
                .Get<RabbitMqConf>() ??
                throw new AppConfigurationException("RabbitMq Configuration");

            services.AddMassTransit(conf =>
            {
                conf.SetKebabCaseEndpointNameFormatter();

                conf.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(rabbitMqConf.Host, h =>
                    {
                        h.Username(rabbitMqConf.User);
                        h.Password(rabbitMqConf.Password);
                    });

                    configurator.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
