using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Feedback;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackEntity.Specifications;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Queries.FeedbackEntity.GetFeedbackWithOwner
{
    public class GetFeedbackWithOwnerHandler :
        IRequestHandler<GetFeedbackWithOwnerQuery, FeedbackResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBlobService blobService;

        public GetFeedbackWithOwnerHandler(
            IUnitOfWork unitOfWork,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
        }

        public async Task<FeedbackResponse> Handle(
            GetFeedbackWithOwnerQuery request, 
            CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackByCriteria(
                new FeedbackByIdWithOwnerSpecification(request.Id), 
                cancellationToken);
        
            if(feedback is null)
            {
                throw new NotFoundException("Feedback doesnt exist");
            }

            var feedbackImages = await blobService.GetBlobFolder(
                feedback.ImageFolder,
                cancellationToken);

            // TODO in this situation maybe automapper could be helpful
            return new FeedbackResponse 
            { 
                Id = feedback.Id,
                Rating = feedback.Rating,
                SendTime = feedback.SendTime,
                UpdateTime = feedback.UpdateTime,
                Text = feedback.Text,
                TestId = feedback.TestId,
                FeedbackImages = feedbackImages.ToList(),
                Profile = new BaseProfileResponse
                {
                    Id = feedback.Owner.Id,
                    Email = feedback.Owner.Email,
                    Name = feedback.Owner.Name,
                }
            };

        }
    }
}
