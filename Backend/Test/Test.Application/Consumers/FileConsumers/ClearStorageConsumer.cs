using MassTransit;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.File;

namespace Test.Application.Consumers.FileConsumers
{
    public class ClearStorageConsumer : IConsumer<DeleteFilesFromStorage>
    {
        private readonly IBackgroundJobService backgroundJobService;

        public ClearStorageConsumer(
            IBackgroundJobService backgroundJobService)
        {
            this.backgroundJobService = backgroundJobService;
        }

        public Task Consume(ConsumeContext<DeleteFilesFromStorage> context)
        {
            foreach(var filePath in context.Message.PathFiles)
            {
                backgroundJobService.CreateFireAndForgetJob<IBlobService>(x => 
                    x.DeleteBlobFolder(filePath, CancellationToken.None));
            }

            return Task.CompletedTask;
        }
    }
}
