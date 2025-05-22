using Application.Shared.Exceptions;
using Authorization.API.Configurations;
using Authorization.API.Validators.EmailCode;
using Authorization.API.Validators.UserEntity;
using Authorization.Application.Commands.Email.SendConfirmCode;
using Authorization.Application.Commands.Email.VerifyCode;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Sinks.Network;
using System.Threading.RateLimiting;

namespace Authorization.API.Extension
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddLogstashLoging(configuration)
                .AddFlientValidation()
                .AddRateLimits();

            return services;
        }

        private static IServiceCollection AddFlientValidation(
            this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<RegisterCommand>, RegisterUserValidator>();
            services.AddScoped<IValidator<LoginCommand>, LoginUserValidator>();

            services.AddScoped<IValidator<SendConfirmCodeCommand>, SendEmailConfirmCodeValidator>();
            services.AddScoped<IValidator<VerifyCodeCommand>, VerifyCodeValidator>();

            return services;
        }

        private static IServiceCollection AddRateLimits(
            this IServiceCollection services)
        {
            const int RATE_LIMIT = 60;

            services.AddRateLimiter(options =>
             {
                 options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                 options.AddPolicy("confirm_email", httpContext =>
                     RateLimitPartition.GetFixedWindowLimiter(
                         partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                         factory: partition => new FixedWindowRateLimiterOptions
                         {
                             Window = TimeSpan.FromSeconds(RATE_LIMIT),
                             PermitLimit = 1
                         })
                 );
             });

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
