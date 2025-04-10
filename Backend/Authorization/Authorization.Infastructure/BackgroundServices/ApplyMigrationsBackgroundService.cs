using Authorization.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Authorization.Infastructure.BackgroundServices
{
    public class ApplyMigrationsBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public ApplyMigrationsBackgroundService(
            IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitDatabase(stoppingToken);
            var scope = scopeFactory.CreateAsyncScope();
            var migrationService = scope.ServiceProvider
                .GetRequiredService<IMigrationService>();
            var logger = scope.ServiceProvider
                .GetRequiredService<Logger<ApplyMigrationsBackgroundService>>();

            var pendingMigrations = await migrationService.GetPendingMigrations();

            if (pendingMigrations.Any())
            {
                await migrationService.ApplyPendingMigrations();
                logger.LogInformation("Apply Migrations");
            }

            logger.LogInformation("Migrations already replied");
        }

        private async Task WaitDatabase(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateAsyncScope();
            var unitOfWork = scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();

            while(!cancellationToken.IsCancellationRequested)
            {

                if(await unitOfWork.CanConnectAsync())
                {
                    return;
                }

                await Task.Delay(3000, cancellationToken);
            }
        }
    }
}
