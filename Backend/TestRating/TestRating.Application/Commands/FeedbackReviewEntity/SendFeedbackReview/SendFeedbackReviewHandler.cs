using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Queries.FeedbackReviewEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.FeedbackReviewEntity.SendFeedbackReview
{
    public class SendFeedbackReviewHandler :
        IRequestHandler<SendFeedbackReviewCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public SendFeedbackReviewHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            SendFeedbackReviewCommand request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfileById(request.ProfileId, cancellationToken);

            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackById(request.FeedbackId, cancellationToken);

            if(feedback is null)
            {
                throw new BadRequestException("Feedback doesnt exist");
            }

            var profileSentReview = await unitOfWork.ReviewRepository
                .GetFeedbackReviewByCriteria(new ReviewProfileToFeedbackSpecification(
                    request.FeedbackId, 
                    request.ProfileId), 
                cancellationToken);

            if(profileSentReview is null)
            {
                var review = FeedbackReview.Initialize(
                    request.IsPositive, 
                    request.ProfileId, 
                    request.FeedbackId);
            
                if(review.IsFailure)
                {
                    throw new BadRequestException("Invalid request value - " + 
                        review.Error);
                }

                await unitOfWork.ReviewRepository
                    .AddReview(review.Value, cancellationToken);
            }
            else if(profileSentReview.IsPositive != request.IsPositive)
            {
                profileSentReview.ChangeReview(request.IsPositive);

                await unitOfWork.ReviewRepository
                    .UpdateReview(profileSentReview.Id, profileSentReview, cancellationToken);
            }
            else
            {
                await unitOfWork.ReviewRepository
                    .DeleteReview(profileSentReview.Id, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
