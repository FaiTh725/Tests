using DnsClient.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Test.Application.Common.Interfaces;
using Test.Application.Common.Wrappers;
using Test.Domain.Interfaces;

namespace Test.Infrastructure.BackgroundServices
{
    public class OutboxBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        private const int OutboxProcessorFrequency = 7;

        public OutboxBackgroundService(
            IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var logger = scope.ServiceProvider
                .GetRequiredService<ILogger<OutboxBackgroundService>>();
            var backgroundJobService = scope.ServiceProvider
                .GetRequiredService<IBackgroundJobService>();

            logger.LogInformation("Starting OutboxBackgroundService");

            while(!stoppingToken.IsCancellationRequested)
            {
                backgroundJobService.CreateFireAndForgetJob<IMessagePublisher>(x =>
                    x.PublishPendingMessages());

                await Task.Delay(TimeSpan.FromSeconds(OutboxProcessorFrequency), stoppingToken);
            }

            logger.LogInformation("OutboxBackgroundService finished");
        }
    }
}
