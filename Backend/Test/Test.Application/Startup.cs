using Microsoft.Extensions.DependencyInjection;

namespace Test.Application
{
    public static class Startup
    {
        public static IServiceCollection ConfigureAppServices(
            this IServiceCollection services)
        {

            return services;
        }

        public static IServiceCollection AddMediatorProvider(
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
