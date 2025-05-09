using Application.Shared.Exceptions;
using MassTransit;
using MediatR;
using Test.Application.Contracts.File;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.Question.DeleteQuestion
{
    public class DeleteQuestionHandler :
        IRequestHandler<DeleteQuestionCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IOutboxService outboxService;

        public DeleteQuestionHandler(
            INoSQLUnitOfWork unitOfWork,
            IOutboxService outboxService)
        {
            this.unitOfWork = unitOfWork;
            this.outboxService = outboxService;
        }

        public async Task Handle(
            DeleteQuestionCommand request, 
            CancellationToken cancellationToken)
        {
            var question = await unitOfWork.QuestionRepository
                .GetQuestion(request.QuestionId, cancellationToken);

            if(question is null)
            {
                throw new NotFoundException("Question doesnt exist");
            }

            using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await unitOfWork.QuestionRepository
                    .DeleteQuestion(request.QuestionId, transaction, cancellationToken);

                await outboxService.AddOutboxMessage(new DeleteFilesFromStorage
                {
                    PathFiles = [question.ImageFolder]
                },
                transaction,
                cancellationToken);

                question.Delete();
                unitOfWork.TrackEntity(question);

                await unitOfWork.CommitTransactionAsync(transaction, cancellationToken);
            }
            catch
            {
                await unitOfWork.RollBackTransactionAsync(transaction, cancellationToken);
                throw;
            }
        }
    }
}
