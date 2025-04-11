using Authorization.Dal.Implementations;
using Authorization.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Dal
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDalServices(
            this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMigrationService, MigrationService>();

            return services;
        }
    }
}
