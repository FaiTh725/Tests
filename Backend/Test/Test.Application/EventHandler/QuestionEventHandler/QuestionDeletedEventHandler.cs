using MediatR;
using Microsoft.Extensions.Logging;
using Test.Application.Contracts.File;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Domain.Events;
using Test.Domain.Interfaces;

namespace Test.Application.EventHandler.QuestionEventHandler
{
    public class QuestionDeletedEventHandler :
        INotificationHandler<QuestionDeletedEvent>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly ILogger<QuestionDeletedEventHandler> logger;
        private readonly IOutboxService outboxService;

        public QuestionDeletedEventHandler(
            INoSQLUnitOfWork unitOfWork,
            ILogger<QuestionDeletedEventHandler> logger,
            IOutboxService outboxService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.outboxService = outboxService;
        }

        public async Task Handle(
            QuestionDeletedEvent notification, 
            CancellationToken cancellationToken)
        {
            var questionAnswer = await unitOfWork.QuestionAnswerRepository
                .GetQuestionAnswersByCriteria(
                new AnswersByQuestionIdSpecification(notification.QuestionId), 
                cancellationToken);

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await unitOfWork.QuestionAnswerRepository.DeleteAnswers(
                        questionAnswer
                            .Select(x => x.Id)
                            .ToList(), 
                        cancellationToken);

                var imagesFolderToDelete = questionAnswer
                        .Select(x => x.ImageFolder)
                        .ToList();

                await outboxService.AddOutboxMessage(new DeleteFilesFromStorage
                {
                    PathFiles = questionAnswer
                        .Select(x => x.ImageFolder)
                        .ToList()
                }, 
                cancellationToken);

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.LogInformation("QuestionDeleted event handler executed");
            }
            catch(Exception ex)
            {
                logger.LogError("Error execute question deleted event handler. " +
                    ex.Message);

                await unitOfWork.RollBackTransactionAsync(cancellationToken);
            }
        }
    }
}
