using Application.Shared.Exceptions;
using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Consumers.ProfileConsumers;
using TestRating.Application.Contacts.Profile;
using TestRating.Infrastructure.Configurations;
using TestRating.Infrastructure.Implementations;
using static CSharpFunctionalExtensions.Result;

namespace TestRating.Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection ConfigureInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddMasstransitProvider(configuration)
                .AddJwtAuthorization(configuration)
                .AddAzuriteProvider(configuration);

            services.AddSingleton<ITokenService<ProfileToken>, ProfileTokenService>();
            services.AddSingleton<IBlobService, AzuriteBlobStorageService>();

            return services;
        }

        private static IServiceCollection AddAzuriteProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var azuriteConnection = configuration
                .GetConnectionString("AzuriteBlobStorage") ??
                throw new AppConfigurationException("Azurite connection string");

            services.AddSingleton(new BlobServiceClient(azuriteConnection));

            return services;
        }

        private static IServiceCollection AddJwtAuthorization(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtConf = configuration
                .GetSection("JwtSettings")
                .Get<JwtConf>() ??
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

                            if (!string.IsNullOrEmpty(token))
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

        private static IServiceCollection AddMasstransitProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var rabbitMqConf = configuration
                .GetSection("RabbitMqSettings")
                .Get<RabbitMqConf>() ??
                throw new AppConfigurationException("RabbitMq configuration setting");

            services.AddMassTransit(conf =>
            {
                conf.SetKebabCaseEndpointNameFormatter();

                conf.AddConsumer<CreateFeedbackProfileConsumer>();
                conf.AddConsumer<DeleteFeedbackProfileConsumer>();

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
