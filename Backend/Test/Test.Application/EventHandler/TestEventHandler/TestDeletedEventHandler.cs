using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Test.Application.Contracts.File;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Application.Queries.QuestionEntity.Specifications;
using Test.Domain.Events;
using Test.Domain.Interfaces;

namespace Test.Application.EventHandler.TestEventHandler
{
    public class TestDeletedEventHandler : INotificationHandler<TestDeletedEvent>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly ILogger<TestDeletedEventHandler> logger;
        private readonly IPublishEndpoint bus;

        public TestDeletedEventHandler(
            INoSQLUnitOfWork unitOfWork,
            ILogger<TestDeletedEventHandler> logger,
            IPublishEndpoint bus)
        {
            this.unitOfWork = unitOfWork;
            this.bus = bus;
        }

        public async Task Handle(
            TestDeletedEvent notification, 
            CancellationToken cancellationToken)
        {
            var testQuestion = await unitOfWork.QuestionRepository
                .GetQuestionsByCriteria(
                new QuestionsByTestIdSpecification(notification.TestId), 
                cancellationToken);
            
            var testQuestionIdList = testQuestion
                        .Select(x => x.Id)
                        .ToList();

            var questionAnswers = await unitOfWork.QuestionAnswerRepository
                .GetQuestionAnswersByCriteria(
                    new AnswersByQuestionsIdSpecification(testQuestionIdList),
                    cancellationToken);

            var questionAnswersIdList = questionAnswers
                .Select(x => x.Id)
                .ToList();

            var imagesFolderToDelete = testQuestion
                .Select(x => x.ImageFolder)
                .ToList();

            imagesFolderToDelete.AddRange(questionAnswers
                .Select(x => x.ImageFolder));

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            
            try
            {
                await unitOfWork.QuestionRepository
                    .DeleteQuestions(testQuestionIdList, cancellationToken);

                await unitOfWork.QuestionAnswerRepository
                    .DeleteAnswers(questionAnswersIdList, cancellationToken);

                await bus.Publish(new DeleteFilesFromStorage
                {
                    PathFiles = imagesFolderToDelete
                }, cancellationToken);

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.LogInformation("TestDeleted event handler executed");
            }
            catch(Exception ex)
            {
                logger.LogError("Error clear data after deleting test. " +
                    $"Error message: {ex.Message}");   

                await unitOfWork.RollBackTransactionAsync(cancellationToken);
            }
        }
    }
}
