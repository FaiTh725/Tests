using Microsoft.Extensions.DependencyInjection;
using Test.Application.Behaviors;

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
                cfg.AddOpenBehavior(typeof(DomainEventsDispatcherBehavior<,>));
                cfg.AddOpenBehavior(typeof(OwnerAndAdminTestAccessBehavior<,>));
                cfg.AddOpenBehavior(typeof(OwnerAndAdminQuestionAccessBehavior<,>));
            });

            return services;
        }
    }
}
