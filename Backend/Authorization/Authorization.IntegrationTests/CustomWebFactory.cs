using Authorization.Application.SagaOrchestrator;
using Authorization.Application.SagaOrchestrator.States;
using Authorization.Dal;
using Authorization.Domain.Entities;
using Authorization.Domain.Validators;
using Authorization.IntegrationTests.Configurations;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Data.Common;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Authorization.IntegrationTests
{
    public class CustomWebFactory : 
        WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
                        .WithImage("postgres:latest")
                        .WithDatabase("Authorize")
                        .WithUsername("admin")
                        .WithPassword("admin")
                        .WithCleanUp(true)
                        .Build();

        private readonly RedisContainer redisContainer = new RedisBuilder()
                        .WithImage("redis/redis-stack:latest")
                        .WithCleanUp(true)
                        .Build();

        private readonly RabbitMqContainer rabbitMqContainer = new RabbitMqBuilder()
                        .WithImage("rabbitmq:management")
                        .WithUsername("guest")
                        .WithPassword("guest")
                        .WithCleanUp(true)
                        .Build();

        public string DbConnectionString = string.Empty;
        public string RedisConnection = string.Empty;

        public async Task InitializeAsync()
        {
            await dbContainer.StartAsync();
            await redisContainer.StartAsync();
            await rabbitMqContainer.StartAsync();

            DbConnectionString = dbContainer.GetConnectionString();
            RedisConnection = redisContainer.GetConnectionString();

            Environment.SetEnvironmentVariable("ConnectionStrings:NpgConnection", DbConnectionString);
            Environment.SetEnvironmentVariable("ConnectionStrings:RedisCacheConnection", RedisConnection);

            Environment.SetEnvironmentVariable("RabbitMqSettings:Host", new Uri(rabbitMqContainer.GetConnectionString()).ToString());
            Environment.SetEnvironmentVariable("RabbitMqSettings:Password", "guest");
            Environment.SetEnvironmentVariable("RabbitMqSettings:User", "guest");

            await RunMigrations();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var dbDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>));
                var dbConnectionDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(DbConnection));
                var redisDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(IConnectionMultiplexer));
                var massTransitdescriptors = services
                    .Where(x => x.ServiceType.Namespace?.StartsWith("MassTransit") == true)
                    .ToList();

                if(dbDescriptor is not null)
                {
                    services.Remove(dbDescriptor);
                }

                if(dbConnectionDescriptor is not null)
                {
                    services.Remove(dbConnectionDescriptor);
                }

                if(redisDescriptor is not null)
                {
                    services.Remove(redisDescriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(DbConnectionString);
                });

                foreach(var massTransitDescriptor in massTransitdescriptors)
                {
                    services.Remove(massTransitDescriptor);
                }

                var redisConnection = ConnectionMultiplexer
                    .Connect(RedisConnection);
                services.AddSingleton<IConnectionMultiplexer>(redisConnection);

                services.AddMassTransitTestHarness(conf =>
                {
                    conf.SetKebabCaseEndpointNameFormatter();

                    conf.AddSagaStateMachine<RegisterUserSaga, RegisterUserSagaState>()
                        .InMemoryRepository();

                    conf.UsingRabbitMq((context, configurator) =>
                    {
                        configurator.Host(new Uri(rabbitMqContainer.GetConnectionString()), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        configurator.ConfigureEndpoints(context);
                    });
                });
            });
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await dbContainer.StopAsync();
            await redisContainer.StopAsync();
            await rabbitMqContainer.StopAsync();
        }

        private async Task RunMigrations()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(DbConnectionString)
                .Options;

            using var context = new AppDbContext(options);
            await context.Database.MigrateAsync();
        }
    }
}
