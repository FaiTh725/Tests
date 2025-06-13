using MassTransit;
using Microsoft.Extensions.Logging;
using Test.Application.Contracts.File;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Application.Queries.QuestionEntity.Specifications;
using Test.Contracts.TestEntity;
using Test.Domain.Interfaces;

namespace Test.Application.Consumers.TestConsumers
{
    public class DeleteDependentsTestEntitiesConsumer :
        IConsumer<DeleteDependentsTestEntities>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly ILogger<DeleteDependentsTestEntitiesConsumer> logger;
        private readonly IOutboxService outboxService;

        public DeleteDependentsTestEntitiesConsumer(
            INoSQLUnitOfWork unitOfWork,
            ILogger<DeleteDependentsTestEntitiesConsumer> logger,
            IOutboxService outboxService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.outboxService = outboxService;
        }

        public async Task Consume(
            ConsumeContext<DeleteDependentsTestEntities> context)
        {
            var testQuestion = await unitOfWork.QuestionRepository
                .GetQuestionsByCriteria(
                new QuestionsByTestIdSpecification(context.Message.TestId),
                context.CancellationToken);

            var testQuestionIdList = testQuestion
                        .Select(x => x.Id)
                        .ToList();

            var questionAnswers = await unitOfWork.QuestionAnswerRepository
                .GetQuestionAnswersByCriteria(
                    new AnswersByQuestionsIdSpecification(testQuestionIdList),
                    context.CancellationToken);

            var questionAnswersIdList = questionAnswers
                .Select(x => x.Id)
                .ToList();

            var imagesFolderToDelete = testQuestion
            .Select(x => x.ImageFolder)
                .ToList();

            imagesFolderToDelete.AddRange(questionAnswers
            .Select(x => x.ImageFolder));

            using var transaction = await unitOfWork.BeginTransactionAsync(context.CancellationToken);

            try
            {
                await unitOfWork.QuestionRepository
                    .DeleteQuestions(testQuestionIdList, transaction, context.CancellationToken);

                await unitOfWork.QuestionAnswerRepository
                    .DeleteAnswers(questionAnswersIdList, transaction, context.CancellationToken);

                await outboxService.AddOutboxMessage(new DeleteFilesFromStorage
                {
                    PathFiles = imagesFolderToDelete
                },
                transaction,
                context.CancellationToken);

                await unitOfWork.CommitTransactionAsync(transaction, context.CancellationToken);

                logger.LogInformation("Delete Dependents Test Entities consumer executed");
            }
            catch (Exception ex)
            {
                logger.LogError("Error clear data after deleting test. " +
                    $"Error message: {ex.Message}");

                await unitOfWork.RollBackTransactionAsync(transaction, context.CancellationToken);
            }
        }
    }
}
