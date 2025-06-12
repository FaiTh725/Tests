using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestRating.Domain.Interfaces;

namespace TestRating.Infrastructure.BackgroundServices
{
    public class ApplyMigrationsBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ApplyMigrationsBackgroundService(
            IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitDatabase(stoppingToken);
            var scope = serviceScopeFactory.CreateAsyncScope();
            var migrationService = scope.ServiceProvider
                .GetRequiredService<IMigrationService>();
            var logger = scope.ServiceProvider
                .GetRequiredService<ILogger<ApplyMigrationsBackgroundService>>();

            var pendingMigrations = await migrationService
                .GetPendingMigrations(stoppingToken);
        
            if(pendingMigrations.Any())
            {
                await migrationService.ApplyPendingMigrations(stoppingToken);
                logger.LogInformation("Apply pending migrations");
            }
            else
            {
                logger.LogInformation("Migrations already applied");
            }
        }

        private async Task WaitDatabase(CancellationToken cancellationToken = default)
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var unitOfWork = scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();
        
            while(!cancellationToken.IsCancellationRequested)
            {
                if(await unitOfWork.CanConnectAsync(cancellationToken))
                {
                    return;
                }

                await Task.Delay(3000, cancellationToken);
            }
        }
    }
}
