using Azure.Storage.Blobs;
using DotNet.Testcontainers.Builders;
using Hangfire;
using Hangfire.InMemory;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Hangfire.States;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Redis.OM;
using System.Linq.Expressions;
using Test.Application.Consumers.FileConsumers;
using Test.Application.Consumers.ProfileConsumers;
using Test.Application.Consumers.QuestionConsumers;
using Test.Application.Consumers.TestConsumers;
using Test.Infrastructure.BackgroundServices;
using Test.Infrastructure.RedisEntities;
using Test.Integration.Tests.Consumers;
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
                        .WithUsername("")
                        .WithPassword("")
                        .WithCommand("--replSet", "rs0", "--bind_ip_all")
                        .WithCleanUp(true)
                        .Build();

        private readonly RedisContainer redisContainer = new RedisBuilder()
                        .WithImage("redis/redis-stack:latest")
                        .WithCleanUp(true)
                        .WithPortBinding(16379, 6379)
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
            RedisConnectionString = $"redis://{redisContainer.Hostname}:{redisContainer.GetMappedPublicPort(6379)}"; 
            AzuriteConnectionString = azuriteContainer.GetConnectionString();

            Environment.SetEnvironmentVariable("ConnectionStrings:MongoDbConnection", DbConnectionString);
            Environment.SetEnvironmentVariable("ConnectionStrings:RedisConnection", RedisConnectionString);
            Environment.SetEnvironmentVariable("ConnectionStrings:AzuriteBlobStorage", AzuriteConnectionString);

            Environment.SetEnvironmentVariable("RabbitMqSettings:Host", new Uri(rabbitMqContainer.GetConnectionString()).ToString());
            Environment.SetEnvironmentVariable("RabbitMqSettings:Password", "guest");
            Environment.SetEnvironmentVariable("RabbitMqSettings:User", "guest");
            
            await CreateIndexes();
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
                    .SingleOrDefault(x => x.ServiceType == typeof(RedisConnectionProvider));

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

                if(redisDescriptor is not null)
                {
                    services.Remove(redisDescriptor);
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

                services.AddSingleton(new RedisConnectionProvider(RedisConnectionString));

                // TODO: refactoring
                ConfigureTestAuthPolicy(services);
                MockHangFire(services);
                RemoveBackgroundServices(services);

                services.AddMassTransitTestHarness(conf =>
                {
                    conf.SetKebabCaseEndpointNameFormatter();

                    conf.AddConsumer<ClearStorageConsumer>();
                    conf.AddConsumer<CreateTestProfileConsumer>();
                    conf.AddConsumer<DeleteTestProfileConsumer>();
                    conf.AddConsumer<DeleteDependentsTestEntitiesConsumer>();
                    conf.AddConsumer<DeleteDependentsQuestionEntitiesConsumer>();
                    
                    conf.AddConsumer<MessagesConsumer>();

                    conf.SetTestTimeouts(testTimeout: TimeSpan.FromSeconds(3));
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

        private void MockHangFire(IServiceCollection services)
        {
            var hangfireDescriptors = services
                .Where(x => x.ServiceType.FullName?
                    .Contains("hangfire", StringComparison.InvariantCultureIgnoreCase) == true)
                .ToList();
            
            foreach(var descriptor in hangfireDescriptors)
            {
                services.Remove(descriptor);
            }

            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            services.AddHangfire(x =>
            {

                x.UseSimpleAssemblyNameTypeSerializer()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseInMemoryStorage()
                .UseSerializerSettings(jsonSettings);
            });
            services.AddHangfireServer();
        }

        private void RemoveBackgroundServices(IServiceCollection services)
        {
            var clearInactiveSessionDescriptor = services
                .SingleOrDefault(x => x.ImplementationType == typeof(ClearInactiveSessionsBackgroundService));
            var outboxBackgroundServiceDescriptor = services
                .SingleOrDefault(x => x.ImplementationType == typeof(OutboxBackgroundService));
            var createRedisOmIndexesDescriptor = services
                .SingleOrDefault(x => x.ImplementationType == typeof(CreateRedisOmIndexes));

            if(clearInactiveSessionDescriptor is not null)
            {
                services.Remove(clearInactiveSessionDescriptor);
            }

            if(outboxBackgroundServiceDescriptor is not null)
            {
                services.Remove(outboxBackgroundServiceDescriptor);
            }

            if(createRedisOmIndexesDescriptor is not null)
            {
                services.Remove(createRedisOmIndexesDescriptor);
            }
        }

        private async Task CreateIndexes()
        {
            using var scope = Services.CreateScope();
            var redisProvider = scope.ServiceProvider
                .GetRequiredService<RedisConnectionProvider>();

            await redisProvider.Connection.CreateIndexAsync(typeof(RedisTestSession));
        }
    }
}
