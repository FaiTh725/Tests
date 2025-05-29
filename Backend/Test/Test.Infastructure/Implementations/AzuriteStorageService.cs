using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.File;

namespace Test.Infrastructure.Implementations
{
    public class AzuriteStorageService : IBlobService
    {
        private const string IMAGE_CONTAINER = "images";
        private readonly BlobContainerClient blobContainer;

        public AzuriteStorageService(
            BlobServiceClient blobServiceClient)
        {
            blobContainer = blobServiceClient.GetBlobContainerClient(IMAGE_CONTAINER);
            blobContainer.CreateIfNotExists();
            blobContainer.SetAccessPolicy(PublicAccessType.Blob);
        }

        public async Task DeleteBlob(string path, CancellationToken cancellationToken = default)
        {
            var blobClient = blobContainer.GetBlobClient(path);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public async Task DeleteBlobFolder(string path, CancellationToken cancellationToken = default)
        {
            var deleteTasks = new List<Task>();
            
            await foreach (var blobItem in blobContainer
                .GetBlobsAsync(
                    prefix: path,
                    cancellationToken: cancellationToken))
            {
                var blobClient = blobContainer.GetBlobClient(blobItem.Name);

                deleteTasks.Add(blobClient
                    .DeleteIfExistsAsync(cancellationToken: cancellationToken));
            }

            await Task.WhenAll(deleteTasks);
        }

        public async Task<IEnumerable<string>> GetBlobFolder(string path, CancellationToken cancellationToken = default)
        {
            var blobUrls = new List<string>();

            await foreach(var blobItem in blobContainer
                .GetBlobsAsync(
                    prefix: path,
                    cancellationToken: cancellationToken))
            {
                var blobClient = blobContainer.GetBlobClient(blobItem.Name);

                blobUrls.Add(blobClient.Uri.AbsoluteUri);
            }

            return blobUrls;
        }

        public async Task<string?> GetBlobUrl(string path, CancellationToken cancellationToken = default)
        {
            var blobClient = blobContainer.GetBlobClient(path);

            if(await blobClient.ExistsAsync(cancellationToken))
            {
                return blobClient.Uri.AbsoluteUri;
            }

            return null;
        }

        public async Task<string> UploadBlob(FileModel file, CancellationToken cancellationToken = default)
        {
            var blobclient = blobContainer.GetBlobClient(file.Name);

            await blobclient.UploadAsync(
                file.Stream,
                new BlobHttpHeaders { ContentType = file.ContentType},
                cancellationToken: cancellationToken);

            return blobclient.Uri.AbsoluteUri;
        }

        public async Task<IEnumerable<string>> UploadBlobs(string pathFolder, List<FileModel> files, CancellationToken cancellationToken = default)
        {
            var uploadFileTasks = files.Select(x => 
                UploadBlob( 
                    new FileModel { 
                        Stream = x.Stream,
                        Name = Path.Combine(pathFolder, x.Name),
                        ContentType = x.ContentType
                    }, 
                    cancellationToken))
                .ToList();

            var blobUrls = await Task.WhenAll(uploadFileTasks);

            return blobUrls;
        }
    }
}
