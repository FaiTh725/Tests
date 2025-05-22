using Application.Shared.Exceptions;
using Authorization.Dal.Implementations;
using Authorization.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Dal
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDalServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var npgConnection = configuration
                .GetConnectionString("NpgConnection") ??
                throw new AppConfigurationException("Postgress ConnectionString");
            
            services.AddDbContext<AppDbContext>(options => 
                options.UseNpgsql(npgConnection));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMigrationService, MigrationService>();

            return services;
        }
    }
}
