using Microsoft.Extensions.DependencyInjection;

namespace Test.Application
{
    public static class Startup
    {
        public static IServiceCollection ConfigureAppServices(
            this IServiceCollection services)
        {
            services.AddMediatorProvider();

            return services;
        }

        private static IServiceCollection AddMediatorProvider(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AssemplyReference).Assembly);
            });

            return services;
        }
    }
}
