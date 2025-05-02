using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Queries.FeedbackEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.FeedbackEntity.SendFeedback
{
    public class SendFeedbackHandler :
        IRequestHandler<SendFeedbackCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBlobService blobService;

        public SendFeedbackHandler(
            IUnitOfWork unitOfWork,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
        }

        public async Task<long> Handle(
            SendFeedbackCommand request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfileById(request.ProfileId, cancellationToken);

            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var sentFeedback = await unitOfWork.FeedbackRepository
                .GetFeedbackByCriteria(
                new FeedbackFromProfileToTestSpecification(request.ProfileId, request.TestId), 
                cancellationToken);

            if(sentFeedback is not null)
            {
                throw new ConflictException("Feedback for this test has already sent, update old");
            }

            var feedbackEntity = Feedback.Initialize(
                request.Text,
                request.TestId,
                request.Rating,
                profile.Id);

            if(feedbackEntity.IsFailure)
            {
                throw new BadRequestException("Invalid request - " + feedbackEntity.Error);
            }

            var feedbackDb = await unitOfWork.FeedbackRepository
                .AddFeedback(feedbackEntity.Value, cancellationToken);

            await blobService.UploadBlobs(
                feedbackDb.ImageFolder, 
                request.FeedbackImages, 
                cancellationToken);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return feedbackDb.Id;
        }
    }
}
