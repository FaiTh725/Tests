using Application.Shared.Exceptions;
using Authorization.Application.SagaOrchestrator;
using Authorization.Application.SagaOrchestrator.States;
using Authorization.Dal;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Validators;
using Authorization.Infrastructure.BackgroundServices;
using Authorization.IntegrationTests.Extensions;
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
using UserRoles = Authorization.Domain.Entities.Role;

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
            var initializeTasks = new List<Task>()
            {
                dbContainer.StartAsync(),
                redisContainer.StartAsync(),
                rabbitMqContainer.StartAsync()
            };

            await Task.WhenAll(initializeTasks);

            DbConnectionString = dbContainer.GetConnectionString();
            RedisConnection = redisContainer.GetConnectionString();

            Environment.SetEnvironmentVariable("ConnectionStrings:NpgConnection", DbConnectionString);
            Environment.SetEnvironmentVariable("ConnectionStrings:RedisCacheConnection", RedisConnection);

            Environment.SetEnvironmentVariable("RabbitMqSettings:Host", new Uri(rabbitMqContainer.GetConnectionString()).ToString());
            Environment.SetEnvironmentVariable("RabbitMqSettings:Password", "guest");
            Environment.SetEnvironmentVariable("RabbitMqSettings:User", "guest");

            await InitializeRoles();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveService(typeof(DbContextOptions<AppDbContext>));
                services.RemoveService(typeof(DbConnection));
                services.RemoveService(typeof(IConnectionMultiplexer));
                services.RemoveServicesByNamespace("MassTransit");

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(DbConnectionString);
                });

                var redisConnection = ConnectionMultiplexer
                    .Connect(RedisConnection);
                services.AddSingleton<IConnectionMultiplexer>(redisConnection);

                services.RemoveService(typeof(InitializeRolesBackgroundService));

                services.AddMassTransitTestHarness(conf =>
                {
                    conf.SetKebabCaseEndpointNameFormatter();

                    conf.AddSagaStateMachine<RegisterUserSaga, RegisterUserSagaState>()
                        .InMemoryRepository();

                    conf.SetTestTimeouts(testTimeout: TimeSpan.FromSeconds(3));
                    
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
            var stopTasks = new List<Task>
            {
                dbContainer.StopAsync(),
                redisContainer.StopAsync(),
                rabbitMqContainer.StopAsync()
            };

            await Task.WhenAll(stopTasks);
        }

        private async Task InitializeRoles() 
        {
            using var scope = Services.CreateScope();
            var unitOfWork = scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();

            var baseRoles = RoleValidator.Roles;
            var existingRoles = await unitOfWork.RoleRepository
                .GetRoles();

            using var transaction = await unitOfWork.BeginTransactionAsync();

            var addRolesTasks = baseRoles.Select(async x =>
            {
                using var innerScope = Services.CreateScope();
                var innerUnitOfWork = innerScope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                var roleDb = existingRoles
                    .FirstOrDefault(role => role.RoleName == x);

                if (roleDb is not null)
                {
                    return;
                }

                var role = UserRoles.Initialize(x);

                if (role.IsFailure)
                {
                    await innerUnitOfWork.RollBackTransactionAsync(transaction);
                    throw new AppConfigurationException("Initialize roles");
                }

                await innerUnitOfWork.RoleRepository.AddRole(role.Value);
                await innerUnitOfWork.SaveChangesAsync();
            }).ToList();

            await Task.WhenAll(addRolesTasks);
            await unitOfWork.CommitTransactionAsync(transaction);
        }
    }
}
