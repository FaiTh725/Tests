using Application.Shared.Exceptions;
using MassTransit;
using MediatR;
using Test.Application.Contracts.File;
using Test.Domain.Intrefaces;

namespace Test.Application.Commands.Question.DeleteQuestion
{
    public class DeleteQuestionHandler :
        IRequestHandler<DeleteQuestionCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IBus bus;

        public DeleteQuestionHandler(
            INoSQLUnitOfWork unitOfWork,
            IBus bus)
        {
            this.unitOfWork = unitOfWork;
            this.bus = bus;
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

            await unitOfWork.QuestionRepository
                .DeleteQuestion(request.QuestionId, cancellationToken);

            await bus.Publish(new DeleteFilesFromStorage
            {
                PathFiles = [question.ImageFolder]
            },
            cancellationToken);

            question.Delete();
            unitOfWork.TrackEntity(question);
        }
    }
}
