using Azure.Storage.Blobs;
using DotNet.Testcontainers.Builders;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
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
using TestRating.Application.Consumers.TestConsumers;
using TestRating.Dal;
using TestRating.Integration.Tests.JwtAuthenticationMock;

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
                var massTransitDescriptors = services
                    .Where(x => x.ServiceType.Namespace?.StartsWith("MassTransit") == true)
                    .ToList();
                var azuriteDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(BlobServiceClient));
                var testExternalServiceDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(ITestExternalService));

                if(testExternalServiceDescriptor is not null)
                {
                    services.Remove(testExternalServiceDescriptor);
                }

                if (dbDescriptor is not null)
                {
                    services.Remove(dbDescriptor);
                }

                if (dbConnectionDescriptor is not null)
                {
                    services.Remove(dbConnectionDescriptor);
                }

                if (azuriteDescriptor is not null)
                {
                    services.Remove(azuriteDescriptor);
                }
                
                foreach (var massTransitDescriptor in massTransitDescriptors)
                {
                    services.Remove(massTransitDescriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(DbConnectionString);
                });

                services.AddSingleton(new BlobServiceClient(AzuriteConnectionString));

                services.AddScoped(_ => TestExternalServiceMock.Object);

                ConfigureTestAuthPolicy(services);

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

        private async Task RunMigrations()
        {
            using var scope = Services.CreateScope();

            using var context = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
        }

        private void ConfigureTestAuthPolicy(IServiceCollection services)
        {
            services.AddTransient<IPolicyEvaluator>(serviceProvider => 
                new TestPolicyEvaluator(ActivatorUtilities
                    .CreateInstance<PolicyEvaluator>(serviceProvider)));

            services
             .AddAuthentication(opts =>
             {
                 opts.DefaultAuthenticateScheme = "Test";
             })
             .AddScheme<JwtBearerOptions, JwtAuthHandler>("Test", opts => { });
            
            services.AddAuthorization(opts =>
            {
                opts.DefaultPolicy = new AuthorizationPolicyBuilder()
                 .AddAuthenticationSchemes("Test")
                 .RequireAuthenticatedUser()
                 .Build();
            });
        }
    }
}
