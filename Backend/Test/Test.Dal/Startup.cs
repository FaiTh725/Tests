using Microsoft.Extensions.DependencyInjection;
using Test.Dal.Services;
using Test.Domain.Intrefaces;

namespace Test.Dal
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDalServices(
            this IServiceCollection services)
        {
            services.AddSingleton<AppDbContext>();

            services.AddScoped<INoSQLUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
