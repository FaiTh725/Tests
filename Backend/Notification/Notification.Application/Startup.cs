using Application.Shared.Exceptions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Behaviors;
using Notification.Application.Configurations;
using Notification.Application.Implementations;
using Notification.Application.Infrastructure.Consumers;
using Notification.Application.Interfaces;

namespace Notification.Application
{
    public static class Startup
    {
        public static IServiceCollection ConfigureAppServices(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            services
                .AddMediatrProvider()
                .AddMasstransitProvider(configuration);

            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IProfileService, ProfileService>();

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

                conf.AddConsumer<SendEmailConsumer>();
                conf.AddConsumer<SendNotificationConsumer>();

                conf.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(rabbitMqConf.Host, h =>
                    {
                        h.Username(rabbitMqConf.User);
                        h.Password(rabbitMqConf.Password);
                    });

                    configurator.ReceiveEndpoint("send-notification", x =>
                    {
                        x.UseMessageRetry(r => r
                            .Interval(3, TimeSpan.FromSeconds(3)));

                        x.ConfigureConsumer<SendNotificationConsumer>(context, conf =>
                        {
                            conf.UseMessageRetry(r =>
                            {
                                r.Interval(3, TimeSpan.FromSeconds(3));
                                r.Ignore<ApiException>();
                            });
                        });
                    });

                    configurator.ConfigureEndpoints(context);
                });
            });

            return services;
        }

        private static IServiceCollection AddMediatrProvider(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);

                cfg.AddOpenBehavior(typeof(NotifyUserNotificationsChangedBehavior<,>));
            });

            return services;
        }
    }
}
