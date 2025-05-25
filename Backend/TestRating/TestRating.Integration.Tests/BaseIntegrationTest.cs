using Azure.Storage.Blobs;
using MassTransit.Internals;
using MassTransit.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System.Data.Common;
using TestRating.Dal;
using TestRating.Domain.Interfaces;

namespace TestRating.Integration.Tests
{
    public abstract class BaseIntegrationTest :
        IClassFixture<CustomWebFactory>, IAsyncLifetime
    {
        protected readonly CustomWebFactory factory;

        protected IServiceProvider serviceProvider;
        protected ISender sender;
        protected HttpClient client;
        protected AppDbContext context;
        protected IUnitOfWork unitOfWork;
        protected ITestHarness massTransitHarness;
        protected BlobServiceClient blobStorage;

        private DbConnection dbConnection;
        private Respawner respawner;

        protected BaseIntegrationTest(
            CustomWebFactory factory)
        {
            this.factory = factory;

            var scope = factory.Services.CreateScope();
            client = factory.CreateClient();
            massTransitHarness = factory.Services.GetTestHarness();
            serviceProvider = factory.Services;

            context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            sender = scope.ServiceProvider.GetRequiredService<ISender>();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            blobStorage = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        }

        public async Task DisposeAsync()
        {

            await massTransitHarness.InactivityTask;
            await massTransitHarness.Stop();

            await respawner.ResetAsync(dbConnection);
            await dbConnection.CloseAsync();

            await ResetBlobStorage();
        }

        public async Task InitializeAsync()
        {
            dbConnection = new NpgsqlConnection(factory.DbConnectionString);
            await dbConnection.OpenAsync();

            respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore = ["__EFMigrationsHistory"],
                SchemasToInclude = ["public"]
            });

            await massTransitHarness.Start();
        }

        private async Task ResetBlobStorage()
        {
            await foreach(var container in blobStorage.GetBlobContainersAsync())
            {
                var containerClient = blobStorage.GetBlobContainerClient(container.Name);
                await containerClient.CreateIfNotExistsAsync();
                if (await containerClient.ExistsAsync())
                {
                    await containerClient.DeleteAsync();
                }
            }
        }

        // TODO: delete
        private async Task WaitOutboxProcesses()
        {
            const int waitTimeSpanInSeconds = 2;

            var publishedMessages = (await massTransitHarness.Published
                .SelectAsync<object>().ToListAsync())
                .Select(x => x.Context.MessageId)
                .ToHashSet();
            var consumedMessages = (await massTransitHarness.Consumed
                .SelectAsync<object>().ToListAsync())
                .Select(x => x.Context.MessageId)
                .ToHashSet();

            while (!publishedMessages.SetEquals(consumedMessages))
            {
                publishedMessages = (await massTransitHarness.Published
                 .SelectAsync<object>().ToListAsync())
                 .Select(x => x.Context.MessageId)
                 .ToHashSet();
                consumedMessages = (await massTransitHarness.Consumed
                    .SelectAsync<object>().ToListAsync())
                    .Select(x => x.Context.MessageId)
                    .ToHashSet();

                await Task.Delay(TimeSpan.FromSeconds(waitTimeSpanInSeconds));
            }
        }
    }
}
