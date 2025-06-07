using Application.Shared.Exceptions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Network;
using Test.API.Configurations;
using Test.API.Contracts.Question;
using Test.API.Contracts.Test;
using Test.API.Filters;
using Test.API.Validators.Common;
using Test.API.Validators.QuestionValidators;
using Test.API.Validators.TestValidators;
using Test.Application.Contracts.Common;
using Test.Application.Queries.Test.GetTests;

namespace Test.API.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection AddCustomizedSwagger(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "Testing Service API", Version = "v1" });
                o.SchemaFilter<EnumSchemaFilter>();
            });

            return services;
        }

        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddLogstashLoging(configuration)
                .AddGrpcProvider()
                .AddFluentValidation();

            services.AddScoped<VerifyProfileFilter>();

            return services;
        }

        public static void ConfigureHangfireDashBoard(
            this WebApplication app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = [new HangfireAuthorizationFilter()]
            });
        }

        private static IServiceCollection AddGrpcProvider(
            this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });

            return services;
        }

        private static IServiceCollection AddFluentValidation(
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
