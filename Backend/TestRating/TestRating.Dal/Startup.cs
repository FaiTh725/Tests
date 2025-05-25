using Application.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestRating.Dal.Interceptors;
using TestRating.Dal.Services;
using TestRating.Domain.Interfaces;

namespace TestRating.Dal
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDalServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var postgressConnection = configuration
                .GetConnectionString("PostgressConnection") ??
                throw new AppConfigurationException("Postgress connection string");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(postgressConnection);
            });

            services.AddScoped<SoftDeleteInterceptor>();
            services.AddScoped<DomainEventsInterceptor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMigrationService, MigrationService>();

            return services;
        }
    }
}
