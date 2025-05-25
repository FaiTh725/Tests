using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using StackExchange.Redis;
using Test.Application.Consumers.FileConsumers;
using Test.Application.Consumers.ProfileConsumers;
using Test.Application.Consumers.TestConsumers;
using Test.Integration.Tests.JwtAuthenticationMock.cs;
using Testcontainers.Azurite;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Test.Integration.Tests
{
    public class CustomWebFactory :
        WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MongoDbContainer dbContainer = new MongoDbBuilder()
                        .WithImage("mongo:latest")
                        .WithUsername("admin")
                        .WithPassword("admin")
                        .WithReplicaSet()
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

        public async Task InitializeAsync()
        {
            var initializeTasks = new List<Task>
            {
                dbContainer.StartAsync(),
                redisContainer.StartAsync(),
                rabbitMqContainer.StartAsync(),
                azuriteContainer.StartAsync(),
            };

            await Task.WhenAll(initializeTasks);

            await dbContainer.ExecScriptAsync("rs.initiate();");

            DbConnectionString = dbContainer.GetConnectionString();
            RedisConnectionString = redisContainer.GetConnectionString();
            AzuriteConnectionString = azuriteContainer.GetConnectionString();

            Environment.SetEnvironmentVariable("ConnectionStrings:MongoDbConnection", DbConnectionString);
            Environment.SetEnvironmentVariable("ConnectionStrings:AzuriteBlobStorage", AzuriteConnectionString);
            Environment.SetEnvironmentVariable("ConnectionStrings:RedisConnection", RedisConnectionString);

            Environment.SetEnvironmentVariable("RabbitMqSettings:Host", new Uri(rabbitMqContainer.GetConnectionString()).ToString());
            Environment.SetEnvironmentVariable("RabbitMqSettings:Password", "guest");
            Environment.SetEnvironmentVariable("RabbitMqSettings:User", "guest");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var dbDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(IMongoClient));
                var databaseDesciptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(IMongoDatabase));
                var massTransitDescriptors = services
                    .Where(x => x.ServiceType.Namespace?.StartsWith("MassTransit") == true)
                    .ToList();
                var azuriteDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(BlobServiceClient));
                var redisDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(IConnectionMultiplexer));

                if (dbDescriptor is not null)
                {
                    services.Remove(dbDescriptor);
                }

                if (databaseDesciptor is not null)
                {
                    services.Remove(databaseDesciptor);
                }

                if (azuriteDescriptor is not null)
                {
                    services.Remove(azuriteDescriptor);
                }

                foreach (var massTransitDescriptor in massTransitDescriptors)
                {
                    services.Remove(massTransitDescriptor);
                }

                var mongoClientSettings = MongoClientSettings
                .FromConnectionString(DbConnectionString);

                var pack = new ConventionPack
                {
                    new EnumRepresentationConvention(BsonType.String)
                };

                ConventionRegistry.Register("EnumStringConvention", pack, _ => true);

                services.AddSingleton<IMongoClient>(new MongoClient(mongoClientSettings));

                services.AddSingleton<IMongoDatabase>(provider => provider
                    .GetRequiredService<IMongoClient>()
                    .GetDatabase("Testing"));

                services.AddSingleton(new BlobServiceClient(AzuriteConnectionString));

                ConfigureTestAuthPolicy(services);

                services.AddMassTransitTestHarness(conf =>
                {
                    conf.SetKebabCaseEndpointNameFormatter();

                    conf.AddConsumer<ClearStorageConsumer>();
                    conf.AddConsumer<CreateTestProfileConsumer>();
                    conf.AddConsumer<DeleteTestProfileConsumer>();
                    conf.AddConsumer<DeleteDependentsTestEntitiesConsumer>();

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
