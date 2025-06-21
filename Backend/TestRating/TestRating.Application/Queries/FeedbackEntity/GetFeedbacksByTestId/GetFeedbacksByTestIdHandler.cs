using MediatR;
using System.Data;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Feedback;
using TestRating.Application.Contacts.Pagination;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackEntity.Specifications;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestId
{
    public class GetFeedbacksByTestIdHandler :
        IRequestHandler<GetFeedbacksByTestIdQuery, BasePaginationResponse<FeedbackWithReviewsResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBlobService blobService;

        public GetFeedbacksByTestIdHandler(
            IUnitOfWork unitOfWork,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
        }

        public async Task<BasePaginationResponse<FeedbackWithReviewsResponse>> Handle(
            GetFeedbacksByTestIdQuery request, 
            CancellationToken cancellationToken)
        {
            var testPaginatedFeedbacks = await unitOfWork.FeedbackRepository
                    .GetPaginatedFeedbacksByCriteria(
                    new FeedbacksPaginationByTestIdWithOwnerAndReviewsSpecification(
                        request.TestId,
                        request.Page,
                        request.PageSize),
                    cancellationToken);

            var getFeedbacksImagesTasks = testPaginatedFeedbacks.Items
                .Select(async x => new FeedbackWithReviewsResponse
                {
                    Id = x.Id,
                    TestId = x.TestId,
                    Text = x.Text,
                    Rating = x.Rating,
                    SendTime = x.SendTime,
                    UpdateTime = x.UpdateTime,
                    Profile = new BaseProfileResponse
                    {
                        Id = x.Owner.Id,
                        Email = x.Owner.Email,
                        Name = x.Owner.Name
                    },
                    CountNegativeReviews = x.Reviews.Count(x => !x.IsPositive),
                    CountPositiveReviews = x.Reviews.Count(x => x.IsPositive),
                    FeedbackImages = (await blobService
                        .GetBlobFolder(x.ImageFolder))
                        .ToList()
                })
                .ToList();

            var feedbacksResponse = await Task.WhenAll(getFeedbacksImagesTasks);

            return new BasePaginationResponse<FeedbackWithReviewsResponse>
            {
                Items = feedbacksResponse,
                Page = request.Page,
                PageCount = request.PageSize,
                MaxCount = testPaginatedFeedbacks.TotalCount
            };
        }
    }
}
