using Application.Shared.Exceptions;
using Azure.Storage.Blobs;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Mongo.Migration.Strategies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileEntity;
using Test.Infrastructure.Configurations;
using Test.Infrastructure.Implementations;
using MassTransit;
using Test.Application.Consumers.FileConsumers;
using Redis.OM;
using Test.Infrastructure.BackgroundServices;
using Test.Application.Contracts.TestSession;

namespace Test.Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection ConfigureInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddAzuriteProvider(configuration)
                .AddJwtAuthorization(configuration)
                .AddHangfireProvider(configuration)
                .AddMasstransitProvider(configuration)
                .AddRedisProvider(configuration);

            services.AddScoped<IBackgroundJobService, HangFireJobService>();
            services.AddScoped<ITempDbService<TempTestSession>, RedisTempDbService>();

            services.AddSingleton<IBlobService, AzuriteStorageService> ();
            services.AddSingleton<ITokenService<ProfileToken>, ProfileTokenService> ();

            services.AddHostedService<CreateRedisOmIndexes>();
            services.AddHostedService<ClearInactiveSessionsBackgroundService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthorization(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtConf = configuration
                .GetSection("JwtSettings")
                .Get<JwtTokenConf>() ??
                throw new AppConfigurationException("Jwt Configuration setting");

            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                jwtOptions =>
                {
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = jwtConf.Audience,
                        ValidIssuer = jwtConf.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                            .GetBytes(jwtConf.SecretKey))
                    };

                    jwtOptions.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            var token = ctx.Request.Cookies["token"];

                            if(!string.IsNullOrEmpty(token))
                            {
                                ctx.Token = token;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddAzuriteProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var azuriteConnection = configuration
                .GetConnectionString("AzuriteBlobStorage") ??
                throw new AppConfigurationException("Azurite connection string");

            services.AddSingleton(new BlobServiceClient(azuriteConnection));

            return services;
        }

        private static IServiceCollection AddHangfireProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var mongoConnectionString = configuration
                .GetConnectionString("MongoDbConnection") ??
                throw new AppConfigurationException("Mongo connection string");

            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var migrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new DropMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            };

            var storageOptions = new MongoStorageOptions
            {
                MigrationOptions = migrationOptions,
                Prefix = "hangfire",
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
            };

            services.AddHangfire(x =>
            {
                x.UseSimpleAssemblyNameTypeSerializer()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseMongoStorage(mongoConnectionString, "HangFire", storageOptions)
                .UseSerializerSettings(jsonSettings);
            });
            services.AddHangfireServer();

            return services;
        }

        private static IServiceCollection AddMasstransitProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var rabbitMqConf = configuration
                .GetSection("RabbitMqSettings")
                .Get<RabbitMqConf>() ??
                throw new AppConfigurationException("RabbitMq configuration");

            services.AddMassTransit(conf =>
            {
                conf.SetKebabCaseEndpointNameFormatter();

                conf.AddConsumer<ClearStorageConsumer>();

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

        private static IServiceCollection AddRedisProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConnection = configuration
                .GetConnectionString("RedisConnection") ??
                throw new AppConfigurationException("Connection string to redis");

            services.AddSingleton(new RedisConnectionProvider(redisConnection));

            return services;
        }
    }
}
