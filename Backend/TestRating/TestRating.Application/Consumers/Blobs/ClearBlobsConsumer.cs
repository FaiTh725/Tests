using MassTransit;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.File;

namespace TestRating.Application.Consumers.Blobs
{
    public class ClearBlobsConsumer :
        IConsumer<ClearBlobFromStorage>
    {
        private readonly IBlobService blobService;

        public ClearBlobsConsumer(
            IBlobService blobService)
        {
            this.blobService = blobService;
        }

        public async Task Consume(ConsumeContext<ClearBlobFromStorage> context)
        {
            var deleteFolderTasks = context.Message.BlobsUrl
                .Select(x => blobService.DeleteBlobFolder(x))
                .ToList();

            await Task.WhenAll(deleteFolderTasks);
        }
    }
}
