using Application.Shared.Exceptions;
using MassTransit;
using MediatR;
using TestRating.Application.Contacts.File;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.FeedbackEntity.DeleteFeedback
{
    public class DeleteFeedbackHandler :
        IRequestHandler<DeleteFeedbackCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBus bus;

        public DeleteFeedbackHandler(
            IUnitOfWork unitOfWork,
            IBus bus)
        {
            this.bus = bus;
            this.unitOfWork = unitOfWork;
        }

        // TODO when deleting feedback any reports can exist
        public async Task Handle(
            DeleteFeedbackCommand request, 
            CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackExcludeFiltersById(request.FeedbackId, cancellationToken);

            if (feedback is null)
            {
                throw new BadRequestException("Feedback doesnt exist");
            }

            // TODO there implement outbox pattern
            await unitOfWork.FeedbackRepository
                .HardDeleteFeedback(feedback.Id, cancellationToken);

            await bus.Publish(new ClearBlobFromStorage
            {
                BlobsUrl = [feedback.ImageFolder]
            }, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
