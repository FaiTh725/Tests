using Azure.Storage.Blobs;
using MassTransit.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Test.Dal;
using Test.Domain.Interfaces;

namespace Test.Integration.Tests
{
    public abstract class BaseIntegrationTest : 
        IClassFixture<CustomWebFactory>, IAsyncLifetime
    {
        protected readonly CustomWebFactory factory;

        protected IServiceProvider serviceProvider;
        protected ISender sender;
        protected HttpClient client;
        protected INoSQLUnitOfWork unitOfWork;
        protected ITestHarness massTransitHarness;
        protected BlobServiceClient blobStorage;
        protected AppDbContext context;

        private IMongoClient mongoClient;
        private IServiceScope scope;

        protected BaseIntegrationTest(
            CustomWebFactory factory)
        {
            this.factory = factory;
        }

        public async Task DisposeAsync()
        {
            await WaitOutboxMessages();

            await massTransitHarness.InactivityTask;
            await massTransitHarness.Stop();

            await ResetBlobStorage();
            await ResetDb();

            scope.Dispose();
        }

        public async Task InitializeAsync()
        {
            scope = factory.Services.CreateScope();
            client = factory.CreateClient();
            massTransitHarness = factory.Services.GetTestHarness();
            serviceProvider = factory.Services;

            mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();

            sender = scope.ServiceProvider.GetRequiredService<ISender>();
            unitOfWork = scope.ServiceProvider.GetRequiredService<INoSQLUnitOfWork>();
            blobStorage = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
            context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await massTransitHarness.Start();
        }

        private async Task ResetBlobStorage()
        {
            await foreach (var container in blobStorage.GetBlobContainersAsync())
            {
                var containerClient = blobStorage.GetBlobContainerClient(container.Name);
                await containerClient.CreateIfNotExistsAsync();
                if (await containerClient.ExistsAsync())
                {
                    await containerClient.DeleteAsync();
                }
            }
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
                if(skipDbToClean.Contains(databaseName))
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

        private async Task WaitOutboxMessages()
        {
            var unreadedMessages = await context.OutboxMessages
                .Find(x => x.ProcessedOnUtc == null)
                .CountDocumentsAsync();
        
            while(unreadedMessages != 0)
            {
                await Task.Delay(3);

                unreadedMessages = await context.OutboxMessages
                .Find(x => x.ProcessedOnUtc == null)
                .CountDocumentsAsync();
            }
        }
    }
}
