using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MassTransit.Internals;
using MassTransit.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using System.Threading;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.TestSession;
using Test.Dal;
using Test.Domain.Interfaces;

namespace Test.Integration.Tests
{
    public abstract class BaseIntegrationTest : IAsyncLifetime
    {
        protected readonly CustomWebFactory factory;

        protected IServiceProvider serviceProvider;
        protected ISender sender;
        protected HttpClient client;
        protected INoSQLUnitOfWork unitOfWork;
        protected ITestHarness massTransitHarness;
        protected BlobServiceClient blobStorage;
        protected AppDbContext context;
        protected ITempDbService<TempTestSession> tempDbService;

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


            await ResetDb();
            await ResetBlobStorage();

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
            unitOfWork = scope.ServiceProvider.GetRequiredService<INoSQLUnitOfWork>();
            blobStorage = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
            context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            tempDbService = scope.ServiceProvider.GetRequiredService<ITempDbService<TempTestSession>>();

            await massTransitHarness.Start();
        }

        private async Task ResetBlobStorage()
        {
            var blobContainer = blobStorage.GetBlobContainerClient("images");
            await blobContainer.CreateIfNotExistsAsync();
            blobContainer.SetAccessPolicy(PublicAccessType.Blob);

            var deleteTasks = new List<Task>();

            await foreach (var blobItem in blobContainer.GetBlobsAsync())
            {
                var blobClient = blobContainer.GetBlobClient(blobItem.Name);
                deleteTasks.Add(blobClient.DeleteIfExistsAsync());
            }

            await Task.WhenAll(deleteTasks);
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

        protected async Task WaitOutboxMessages()
        {
            // it doesnt work
            await massTransitHarness.InactivityTask;
            var publishedMessagesCount = await massTransitHarness.Published
                .SelectAsync<object>()
                .Count();
            var consumedMessagesCount = await massTransitHarness.Consumed
                .SelectAsync<object>()
                .Count();
        
            while(consumedMessagesCount != publishedMessagesCount)
            {
                await Task.Delay(3);

                publishedMessagesCount = await massTransitHarness.Published
                    .SelectAsync<object>()
                    .Count();
                consumedMessagesCount = await massTransitHarness.Consumed
                    .SelectAsync<object>()
                    .Count();
            }
        }
    }
}
