using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test.Application.Commands.Test.ClearTestProgress;
using Test.Application.Common.Interfaces;
using Test.Application.Common.Wrappers;

namespace Test.Infrastructure.BackgroundServices
{
    public class ClearInactiveSessionsBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ClearInactiveSessionsBackgroundService(
            IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = serviceScopeFactory.CreateScope();
            var backgroundJobService = scope.ServiceProvider
                .GetRequiredService<IBackgroundJobService>();

            backgroundJobService.CreateSchedulingJob<MediatorWrapper>(
                "clear_inactive_sessions",
                x => x.SendCommand(new ClearTestProgressCommand()),
                "*/20 * * * *");
        
            return Task.CompletedTask;
        }
    }
}
