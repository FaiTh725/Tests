﻿using Application.Shared.Exceptions;
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
using Test.Infastructure.Configurations;
using Test.Infastructure.Implementations;
using MassTransit;

namespace Test.Infastructure
{
    public static class Startup
    {
        public static IServiceCollection ConfigureInfastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddAzuriteProvider(configuration)
                .AddJwtAuthorization(configuration)
                .AddHangfireProvider(configuration)
                .AddMasstransitProvider(configuration);

            services.AddScoped<IBackgroundJobService, HangFireJobService>();

            services.AddSingleton<IBlobService, AzuriteStorageService> ();
            services.AddSingleton<ITokenService<ProfileToken>, ProfileTokenService> ();

            return services;
        }

        public static IServiceCollection AddJwtAuthorization(
            this IServiceCollection services,
            IConfiguration configurationq)
        {
            var jwtConf = configurationq
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
                .GetConnectionString("MongoDbConnection1") ??
                throw new AppConfigurationException("Posgress connection string");

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
