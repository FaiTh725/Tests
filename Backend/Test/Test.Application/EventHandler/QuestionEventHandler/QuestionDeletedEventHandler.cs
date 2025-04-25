using MassTransit;
using MediatR;
using Test.Application.Contracts.File;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Domain.Events;
using Test.Domain.Intrefaces;

namespace Test.Application.EventHandler.QuestionEventHandler
{
    public class QuestionDeletedEventHandler :
        INotificationHandler<QuestionDeletedEvent>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IBus bus;

        public QuestionDeletedEventHandler(
            INoSQLUnitOfWork unitOfWork,
            IBus bus)
        {
            this.unitOfWork = unitOfWork;
            this.bus = bus;
        }

        public async Task Handle(
            QuestionDeletedEvent notification, 
            CancellationToken cancellationToken)
        {
            var questionAnswer = await unitOfWork.QuestionAnswerRepository
                .GetQuestionAnswersByCriteria(
                new AnswersByQuestionIdSpecification(notification.QuestionId), 
                cancellationToken);

            await unitOfWork.QuestionAnswerRepository.DeleteAnswers(
                    questionAnswer
                        .Select(x => x.Id)
                        .ToList(), 
                    cancellationToken);

            var imagesFolderToDelete = questionAnswer
                    .Select(x => x.ImageFolder)
                    .ToList();

            await bus.Publish(new DeleteFilesFromStorage
            {
                PathFiles = questionAnswer
                    .Select(x => x.ImageFolder)
                    .ToList()
            }, 
            cancellationToken);
        }
    }
}
