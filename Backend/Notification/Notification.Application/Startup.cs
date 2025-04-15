using Application.Shared.Exceptions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Configurations;
using Notification.Application.Implementations;
using Notification.Application.Infastructure.Consumers;
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
                .AddMasstransitProvider(configuration);

            services.AddSingleton<IEmailService, EmailService>();

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
