using Azure.Storage.Blobs;
using MassTransit.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
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
        protected IMongoClient mongoClient;

        protected BaseIntegrationTest(
            CustomWebFactory factory)
        {
            this.factory = factory;

            var scope = factory.Services.CreateScope();
            client = factory.CreateClient();
            massTransitHarness = factory.Services.GetTestHarness();
            serviceProvider = factory.Services;

            sender = scope.ServiceProvider.GetRequiredService<ISender>();
            unitOfWork = scope.ServiceProvider.GetRequiredService<INoSQLUnitOfWork>();
            blobStorage = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
            mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
        }

        public async Task DisposeAsync()
        {
            await massTransitHarness.InactivityTask;
            await massTransitHarness.Stop();

            await ResetBlobStorage();
            await ResetDb();
        }

        public async Task InitializeAsync()
        {
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
            var databases = await mongoClient.ListDatabaseNames()
                .ToListAsync(); 

            foreach (var databaseName in databases)
            {
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
