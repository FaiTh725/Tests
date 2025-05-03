using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Contacts.FeedbackReview;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Queries.FeedbackReviewEntity.GetFeedbackReview
{
    public class GetFeedbackReviewHandler :
        IRequestHandler<GetFeedbackReviewQuery, BaseFeedbackReview>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetFeedbackReviewHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<BaseFeedbackReview> Handle(
            GetFeedbackReviewQuery request, 
            CancellationToken cancellationToken)
        {
            var review = await unitOfWork.ReviewRepository
                .GetReview(request.Id, cancellationToken);

            if(review is null)
            {
                throw new NotFoundException("Review doesnt exist");
            }

            return new BaseFeedbackReview 
            { 
                Id = review.Id,
                IsPositive = review.IsPositive,
                FeedbackId = review.ReviewedFeedbackId,
                OwnerId = review.OwnerId
            };
        }
    }
}
