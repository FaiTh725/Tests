using MassTransit;
using Microsoft.Extensions.Logging;
using Test.Contracts.TestEntity;
using TestRating.Application.Contacts.File;
using TestRating.Application.Queries.FeedbackEntity.Specifications;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Consumers.TestConsumer
{
    public class DeleteTestFeedbacksConsumer :
        IConsumer<DeleteDependentsTestEntities>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<DeleteTestFeedbacksConsumer> logger;
        private readonly IPublishEndpoint bus;

        public DeleteTestFeedbacksConsumer(
            IUnitOfWork unitOfWork,
            ILogger<DeleteTestFeedbacksConsumer> logger,
            IPublishEndpoint bus)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.bus = bus;
        }

        public async Task Consume(
            ConsumeContext<DeleteDependentsTestEntities> context)
        {
            logger.LogInformation("Delete test depends feedbacks consumer starts");

            var feedbacksByTestIdSpecfication = new FeedbacksByTestIdSpecification(context.Message.TestId);

            var testFeedbacks = await unitOfWork.FeedbackRepository
                .GetFeedbacksExcludeFiltersByCriteria(
                feedbacksByTestIdSpecfication,
                context.CancellationToken);

            await bus.Publish(new ClearBlobFromStorage
            {
                BlobsUrl = testFeedbacks
                    .Select(x => x.ImageFolder)
                    .ToList()
            });

            await unitOfWork.FeedbackRepository
                .HardDeleteByCriteria(
                feedbacksByTestIdSpecfication, 
                context.CancellationToken);

            logger.LogInformation("Consumer deleted test feedbacks");
        }
    }
}
