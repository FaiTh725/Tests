using Azure.Storage.Blobs;
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
    public abstract class BaseIntegrationTest : IAsyncLifetime
    {
        protected readonly CustomWebFactory factory;

        protected IServiceProvider serviceProvider;
        protected ISender sender;
        protected HttpClient client;
        protected AppDbContext context;
        protected ITestHarness massTransitHarness;
        protected BlobServiceClient blobStorage;

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

            await ResetBlobStorage();

            scope.Dispose();
        }

        public async Task InitializeAsync()
        {
            scope = factory.Services.CreateScope();
            client = factory.CreateClient();
            massTransitHarness = factory.Services.GetTestHarness();
            serviceProvider = factory.Services;

            context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            sender = scope.ServiceProvider.GetRequiredService<ISender>();
            blobStorage = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();

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
    }
}
