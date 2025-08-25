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
using Notification.Application.Infrastructure.Consumers;
using Notification.IntegrationTests.JwtAuthMock;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;
using static CSharpFunctionalExtensions.Result;

namespace Notification.IntegrationTests
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

        private readonly RabbitMqContainer rabbitMqContainer = new RabbitMqBuilder()
                        .WithImage("rabbitmq:management")
                        .WithUsername("guest")
                        .WithPassword("guest")
                        .WithCleanUp(true)
                        .Build();

        public string DbConnectionString { get; private set; } = string.Empty;

        public async Task InitializeAsync()
        {
            var initializeTasks = new List<Task>
            {
                dbContainer.StartAsync(),
                rabbitMqContainer.StartAsync()
            };

            await Task.WhenAll(initializeTasks);

            await dbContainer.ExecScriptAsync("rs.initiate();");

            DbConnectionString = dbContainer.GetConnectionString();

            Environment.SetEnvironmentVariable("ConnectionStrings:MongoConnection", DbConnectionString);

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
                var databaseDescriptor = services
                    .SingleOrDefault(x => x.ServiceType == typeof(IMongoDatabase));
                var massTransitDescriptors = services
                    .Where(x => x.ServiceType.Namespace?.StartsWith("MassTransit") == true)
                    .ToList();

                if (dbDescriptor is not null)
                {
                    services.Remove(dbDescriptor);
                }

                if(databaseDescriptor is not null)
                {
                    services.Remove(databaseDescriptor);
                }

                foreach (var massTransitDescriptor in massTransitDescriptors)
                {
                    services.Remove(massTransitDescriptor);
                }

                ConfigureMongoDb(services, DbConnectionString);

                ConfigureTestAuthPolicy(services);

                services.AddMassTransitTestHarness(conf =>
                {
                    conf.SetKebabCaseEndpointNameFormatter();

                    conf.AddConsumer<SendEmailConsumer>();
                    conf.AddConsumer<SendNotificationConsumer>();

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
                rabbitMqContainer.StopAsync()
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

        private void ConfigureMongoDb(
            IServiceCollection services, 
            string connectionString)
        {
            var mongoClientSettings = MongoClientSettings
                .FromConnectionString(connectionString);

            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumStringConvention", pack, _ => true);

            services.AddSingleton<IMongoClient>(new MongoClient(mongoClientSettings));

            services.AddSingleton<IMongoDatabase>(provider => provider
            .GetRequiredService<IMongoClient>()
            .GetDatabase("Notification"));
        }
    }
}
