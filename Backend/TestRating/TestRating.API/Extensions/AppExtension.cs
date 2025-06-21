using Application.Shared.Exceptions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Sinks.Network;
using Test.API.Grpc;
using TestRating.API.Configurations;
using TestRating.API.Filters;
using TestRating.API.Grpc.Services;
using TestRating.Application.Common.Constants;
using TestRating.Application.Common.Interfaces;

namespace TestRating.API.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddLogstashLoging(configuration)
                .AddGrpcProvider(configuration)
                .ConfigureFluentValidation();

            services.AddScoped<ITestExternalService, TestExternalService>();
            services.AddScoped<VerifyProfileFilter>();

            services.AddCustomPolicies();

            return services;
        }

        private static IServiceCollection AddGrpcProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var testingServiceUrl = configuration
                .GetValue<string>("ExternalServices:TestingService") ??
                throw new AppConfigurationException("Testing Service Url");

            services.AddGrpcClient<TestService.TestServiceClient>(options =>
            {
                options.Address = new Uri(testingServiceUrl);
            })
            // allow client connect to server with invalid certificate, only for local development
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                return handler;
            });

            return services;
        }

        private static IServiceCollection AddCustomPolicies(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole(UserRoles.Administrator));
            });

            return services;
        }

        private static IServiceCollection ConfigureFluentValidation(
            this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            return services;
        }

        private static IServiceCollection AddLogstashLoging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var logstashConf = configuration
                .GetSection("LogstashSettings")
                .Get<LogstashConf>() ??
                throw new AppConfigurationException("Logstash settings");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.TCPSink(
                    logstashConf.Host,
                    logstashConf.Port,
                    new Serilog.Formatting.Json.JsonFormatter())
                .CreateLogger();

            return services;
        }
    }
}
