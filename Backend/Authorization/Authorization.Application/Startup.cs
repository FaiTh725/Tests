using Authorization.Application.Behaviors;
using Authorization.Application.Common.Implementations;
using Authorization.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Application
{
    public static class Startup
    {
        public static IServiceCollection ConfigureAppServices(
            this IServiceCollection services)
        {
            services.AddMediatRrovider();

            services.AddSingleton<IHashService, HashService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        private static IServiceCollection AddMediatRrovider(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.AddOpenBehavior(typeof(RegistrationAccessBehavior<,>));
                cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
            });

            return services;
        }
    }
}
