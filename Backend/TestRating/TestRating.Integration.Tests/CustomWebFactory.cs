using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data.Common;
using Testcontainers.Azurite;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Consumers.Blobs;
using TestRating.Application.Consumers.ProfileConsumers;
using TestRating.Application.Consumers.TestConsumer;
using TestRating.Dal;
using TestRating.Integration.Tests.Extensions;

namespace TestRating.Integration.Tests
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

        private readonly AzuriteContainer azuriteContainer = new AzuriteBuilder()
                        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
                        .WithCleanUp(true)
                        .Build();

        public string DbConnectionString { get; private set; } = default!;
        public string RedisConnectionString { get; private set; } = default!;
        public string AzuriteConnectionString { get; private set; } = default!;

        public Mock<ITestExternalService> TestExternalServiceMock { get; private set; } = default!;

        public async Task InitializeAsync()
        {
            TestExternalServiceMock = new();

            var initializeTasks = new List<Task>
            {
                dbContainer.StartAsync(),
                redisContainer.StartAsync(),
                rabbitMqContainer.StartAsync(),
                azuriteContainer.StartAsync(),
            };

            await Task.WhenAll(initializeTasks);

            DbConnectionString = dbContainer.GetConnectionString();
            RedisConnectionString = redisContainer.GetConnectionString();
            AzuriteConnectionString = azuriteContainer.GetConnectionString();

            Environment.SetEnvironmentVariable("ConnectionStrings:PostgressConnection", DbConnectionString);
            Environment.SetEnvironmentVariable("ConnectionStrings:AzuriteBlobStorage", AzuriteConnectionString);

            Environment.SetEnvironmentVariable("RabbitMqSettings:Host", new Uri(rabbitMqContainer.GetConnectionString()).ToString());
            Environment.SetEnvironmentVariable("RabbitMqSettings:Password", "guest");
            Environment.SetEnvironmentVariable("RabbitMqSettings:User", "guest");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveService(typeof(DbContextOptions<AppDbContext>));
                services.RemoveService(typeof(DbConnection));
                services.RemoveService(typeof(BlobServiceClient));
                services.RemoveService(typeof(ITestExternalService));
                services.RemoveServicesByNamespace("MassTransit");

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(DbConnectionString);
                });

                services.AddSingleton(new BlobServiceClient(AzuriteConnectionString));

                services.AddScoped(_ => TestExternalServiceMock.Object);

                services.ConfigureTestAuthPolicy();

                services.AddMassTransitTestHarness(conf =>
                {
                    conf.SetKebabCaseEndpointNameFormatter();

                    conf.AddConsumer<CreateFeedbackProfileConsumer>();
                    conf.AddConsumer<DeleteFeedbackProfileConsumer>();
                    conf.AddConsumer<DeleteTestFeedbacksConsumer>();
                    conf.AddConsumer<ClearBlobsConsumer>();

                    conf.AddEntityFrameworkOutbox<AppDbContext>(x =>
                    {
                        x.QueryDelay = TimeSpan.FromMilliseconds(100);
                        x.UsePostgres().UseBusOutbox();
                    });

                    //conf.SetTestTimeouts(testTimeout: TimeSpan.FromSeconds(3));
                    //conf.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));

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
                rabbitMqContainer.StopAsync(),
                azuriteContainer.StopAsync(),
            };

            await Task.WhenAll(stopTasks);
        }
    }
}
