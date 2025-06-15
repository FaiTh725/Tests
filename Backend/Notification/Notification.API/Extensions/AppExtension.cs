using Application.Shared.Exceptions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Notification.API.Configuration;
using Notification.API.Contracts.Notifications;
using Notification.API.Filters;
using Notification.API.Hubs;
using Notification.API.Hubs.Instances;
using Notification.API.Services;
using Notification.API.Validators.Notifications;
using Notification.Application.Interfaces;
using Serilog;
using Serilog.Sinks.Network;
using System.Text;

namespace Notification.API.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddFluentValidatorProvider()
                .AddLogstashLoging(configuration)
                .AddJwtAuthorization(configuration);

            services
                .AddSignalR()
                .AddHubOptions<NotificationHub>(options =>
                {
                    options.AddFilter<HubAuthenticationFilter>();
                });

            services.AddScoped<DecodeTokenFilter>();

            services.AddSingleton<INotificationService, HubNotificationSender>();
            services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();
            services.AddSingleton<HubAuthenticationFilter>();

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

        private static IServiceCollection AddJwtAuthorization(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtConf = configuration
                .GetSection("JwtSettings")
                .Get<JwtConf>() ??
                throw new AppConfigurationException("Jwt Configuration Settings");

            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                jwtOptions =>
                {
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
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

        private static  IServiceCollection AddFluentValidatorProvider(
            this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            services.AddScoped<IValidator<DeleteNotificationsRequest>, DeleteNotificationsValidator>();

            return services;
        }
    }
}
