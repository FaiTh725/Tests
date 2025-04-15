using Authorization.Application.Behaviors;
using Authorization.Application.Common.Implementations;
using Authorization.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Application
{
    public static class Startup
    {
        public static IServiceCollection ConfigureAppSerrvices(
            this IServiceCollection services)
        {
            services.AddMediatRrovider();

            services.AddSingleton<IHashService, HashService>();

            return services;
        }

        private static IServiceCollection AddMediatRrovider(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AssemplyReference).Assembly);
                cfg.AddOpenBehavior(typeof(RegistrationAccessBehavior<,>));
            });

            return services;
        }
    }
}
