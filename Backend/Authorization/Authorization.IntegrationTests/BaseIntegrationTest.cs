using Authorization.Application.Common.Interfaces;
using Authorization.Dal;
using MassTransit.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using StackExchange.Redis;
using System.Data.Common;

namespace Authorization.IntegrationTests
{
    public abstract class BaseIntegrationTest : IAsyncLifetime
    {
        protected readonly CustomWebFactory factory;

        protected ISender sender;
        protected HttpClient client;
        protected ICacheService cache;
        protected AppDbContext context;
        protected ITestHarness massTransitHarness;

        private DbConnection dbConnection;
        private Respawner respawner;
        private IServiceScope scope;

        protected BaseIntegrationTest(
            CustomWebFactory factory)
        {
            this.factory = factory;
        }

        public async Task DisposeAsync()
        {
            await massTransitHarness.InactivityTask;
            await massTransitHarness.Stop();

            await respawner.ResetAsync(dbConnection);
            await dbConnection.CloseAsync();

            await ResetCache();

            scope.Dispose();
        }

        public async Task InitializeAsync()
        {
            scope = factory.Services.CreateScope();
            client = factory.CreateClient();
            massTransitHarness = factory.Services.GetTestHarness();

            cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
            context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            sender = scope.ServiceProvider.GetRequiredService<ISender>();

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

        private async Task ResetCache()
        {
            var redis = await ConnectionMultiplexer
                .ConnectAsync(factory.RedisConnection + ",allowAdmin=true");
            var redisEndpoints = redis.GetEndPoints();

            foreach (var endpoint in redisEndpoints)
            {
                var server = redis.GetServer(endpoint);
                await server.FlushAllDatabasesAsync();
            }
        }
    }
}
