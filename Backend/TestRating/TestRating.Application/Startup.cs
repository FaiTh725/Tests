using Microsoft.Extensions.DependencyInjection;
using TestRating.Application.Behaviors;

namespace TestRating.Application
{
    public static class Startup
    {
        public static IServiceCollection ConfigureAppServices(
            this IServiceCollection services)
        {
            services
                .AddMediatorProvider();

            return services;
        }

        private static IServiceCollection AddMediatorProvider(
            this IServiceCollection services)
        {
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
                
                x.AddOpenBehavior(typeof(CheckTestIsExistBehavior<,>));
                x.AddOpenBehavior(typeof(OwnerAndAdminFeedbackAccessBehavior<,>));
                x.AddOpenBehavior(typeof(OwnerAndAdminReplyAccessBehavior<,>));
            });

            return services;
        }
    }
}
