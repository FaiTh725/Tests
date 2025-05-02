using Application.Shared.Exceptions;
using MassTransit;
using MediatR;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.File;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.FeedbackEntity.ChangeFeedback
{
    public class ChangeFeedbackHandler :
        IRequestHandler<ChangeFeedbackCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBlobService blobService;
        private readonly IBus bus;

        public ChangeFeedbackHandler(
            IUnitOfWork unitOfWork,
            IBlobService blobService,
            IBus bus)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
            this.bus = bus;
        }

        public async Task<long> Handle(
            ChangeFeedbackCommand request, 
            CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackById(request.FeedbackId, cancellationToken);

            if(feedback is null)
            {
                throw new BadRequestException("Feedback doesnt exist");
            }

            feedback.ChangeFeedback(request.Text, request.Rating);
            await unitOfWork.FeedbackRepository
                .UpdateFeedback(feedback.Id, feedback, cancellationToken);

            await bus.Publish(new ClearBlobFromStorage
            {
                BlobsUrl = [feedback.ImageFolder]
            }, cancellationToken);

            await blobService.UploadBlobs(
                feedback.ImageFolder, 
                request.NewImages, 
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return feedback.Id;
        }
    }
}
