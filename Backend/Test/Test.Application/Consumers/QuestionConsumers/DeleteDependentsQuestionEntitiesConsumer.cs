using MassTransit;
using Microsoft.Extensions.Logging;
using Test.Application.Contracts.File;
using Test.Application.Contracts.Question;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Consumers.QuestionConsumers
{
    public class DeleteDependentsQuestionEntitiesConsumer :
        IConsumer<DeleteDependentsQuestionEntities>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ILogger<DeleteDependentsQuestionEntitiesConsumer> logger;

        public DeleteDependentsQuestionEntitiesConsumer(
            INoSQLUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            ILogger<DeleteDependentsQuestionEntitiesConsumer> logger)
        {
            this.unitOfWork = unitOfWork;
            this.publishEndpoint = publishEndpoint;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<DeleteDependentsQuestionEntities> context)
        {
            var questionAnswer = await unitOfWork.QuestionAnswerRepository
            .GetQuestionAnswersByCriteria(
                new AnswersByQuestionIdSpecification(context.Message.QuestionId),
                context.CancellationToken);

            using var transaction = await unitOfWork
                .BeginTransactionAsync(context.CancellationToken);

            try
            {
                await unitOfWork.QuestionAnswerRepository.DeleteAnswers(
                        questionAnswer
                            .Select(x => x.Id)
                            .ToList(),
                        transaction,
                        context.CancellationToken);

                var imagesFolderToDelete = questionAnswer
                        .Select(x => x.ImageFolder)
                        .ToList();

                await publishEndpoint.Publish(new DeleteFilesFromStorage
                {
                    PathFiles = questionAnswer
                    .Select(x => x.ImageFolder)
                    .ToList()
                },
                context.CancellationToken);

                await unitOfWork.CommitTransactionAsync(transaction, context.CancellationToken);

                logger.LogInformation("QuestionDeleted event handler executed");
            }
            catch (Exception ex)
            {
                logger.LogError("Error execute question deleted event handler. " +
                    ex.Message);

                await unitOfWork.RollBackTransactionAsync(transaction, context.CancellationToken);
            }
        }
    }
}
