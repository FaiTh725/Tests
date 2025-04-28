using Microsoft.Extensions.DependencyInjection;
using Test.Application.Behaviors;
using Test.Application.Common.Implementations;
using Test.Application.Common.Interfaces;
using Test.Application.Common.Mediator;

namespace Test.Application
{
    public static class Startup
    {
        public static IServiceCollection ConfigureAppServices(
            this IServiceCollection services)
        {
            services.AddMediatorProvider();

            services.AddScoped<MediatorWrapper>();

            services.AddScoped<ITestEvaluatorService, TestEvaliatorService>();

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
                cfg.AddOpenBehavior(typeof(OwnerAndAdminGroupAccessBehavior<,>));
                cfg.AddOpenBehavior(typeof(TestAccessBehavior<,>));
            });

            return services;
        }
    }
}
