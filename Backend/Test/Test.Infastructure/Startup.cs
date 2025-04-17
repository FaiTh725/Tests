using Application.Shared.Exceptions;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileEntity;
using Test.Infastructure.Configurations;
using Test.Infastructure.Implementations;

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
                .AddJwtAuthorization(configuration);

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
    }
}
