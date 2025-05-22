using Authorization.Application.Common.Interfaces;
using Authorization.Dal;
using Authorization.Domain.Interfaces;
using MassTransit.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using StackExchange.Redis;
using System.Data.Common;

namespace Authorization.IntegrationTests
{
    public abstract class BaseIntegrationTest :
        IClassFixture<CustomWebFactory>, IAsyncLifetime
    {
        protected readonly CustomWebFactory factory;

        protected ISender sender;
        protected HttpClient client;
        protected ICacheService cache;
        protected AppDbContext context;
        protected IUnitOfWork unitOfWork;
        protected ITestHarness massTransitHarness;

        private DbConnection dbConnection;
        private Respawner respawner;

        protected BaseIntegrationTest(
            CustomWebFactory factory)
        {
            this.factory = factory;

            var scope = factory.Services.CreateScope();
            client = factory.CreateClient();
            massTransitHarness = factory.Services.GetTestHarness();

            cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
            context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            sender = scope.ServiceProvider.GetRequiredService<ISender>();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        }

        public async Task DisposeAsync()
        {
            await respawner.ResetAsync(dbConnection);

            var redis = await ConnectionMultiplexer
                .ConnectAsync(factory.RedisConnection + ",allowAdmin=true");
            var redisEndpoints = redis.GetEndPoints();

            foreach (var endpoint in redisEndpoints)
            {
                var server = redis.GetServer(endpoint);
                await server.FlushAllDatabasesAsync();
            }

            await dbConnection.CloseAsync();
            await massTransitHarness.Stop();
        }

        public async Task InitializeAsync()
        {
            dbConnection = new NpgsqlConnection(factory.DbConnectionString);
            await dbConnection.OpenAsync();

            respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore = ["__EFMigrationsHistory", "Roles"],
                SchemasToInclude = ["public"]
            });

            await massTransitHarness.Start();
        }
    }
}
