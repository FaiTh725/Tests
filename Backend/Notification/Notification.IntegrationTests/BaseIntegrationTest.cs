
using MassTransit.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Notification.Infrastructure.Data;

namespace Notification.IntegrationTests
{
    public class BaseIntegrationTest : IAsyncLifetime
    {
        protected readonly CustomWebFactory factory;

        protected IServiceProvider serviceProvider;
        protected ISender sender;
        protected HttpClient client;
        protected AppDbContext context;
        protected ITestHarness massTransitHarness;

        private IMongoClient mongoClient;
        private IServiceScope scope;

        public BaseIntegrationTest(
            CustomWebFactory factory)
        {
            this.factory = factory;
        }

        public async Task DisposeAsync()
        {
            await massTransitHarness.InactivityTask;
            await massTransitHarness.Stop();

            await ResetDb();

            scope.Dispose();
        }

        public async Task InitializeAsync()
        {
            scope = factory.Services.CreateScope();
            client = factory.CreateClient();
            serviceProvider = factory.Services;
            massTransitHarness = scope.ServiceProvider.GetTestHarness();

            mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();

            sender = scope.ServiceProvider.GetRequiredService<ISender>();
            context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await massTransitHarness.Start();
        }

        private async Task ResetDb()
        {
            var databases = await mongoClient
                .ListDatabaseNames()
                .ToListAsync();

            var skipDbToClean = new List<string>
            {
                "admin", "local",
                "config", "HangFire"
            };

            foreach (var databaseName in databases)
            {
                if (skipDbToClean.Contains(databaseName))
                {
                    continue;
                }

                var database = mongoClient.GetDatabase(databaseName);
                var collections = await database.ListCollectionNames().ToListAsync();

                var clearCollectionTasks = collections.Select(async x =>
                {
                    var collection = database.GetCollection<BsonDocument>(x);
                    await collection.DeleteManyAsync(new BsonDocument());
                }).ToList();

                await Task.WhenAll(clearCollectionTasks);
            }
        }
    }
}
