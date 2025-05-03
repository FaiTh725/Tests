using Microsoft.Extensions.DependencyInjection;
using TestRating.Dal.Interceptors;
using TestRating.Dal.Services;
using TestRating.Domain.Interfaces;

namespace TestRating.Dal
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDalServices(
            this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();

            services.AddScoped<SoftDeleteInterceptor>();
            services.AddScoped<DomainEventsInterceptor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMigrationService, MigrationService>();

            return services;
        }
    }
}
