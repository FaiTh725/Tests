using Application.Shared.Exceptions;
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
        private readonly ITestExternalService testExternalService;
        private readonly IBlobService blobService;

        public GetFeedbacksByTestIdHandler(
            IUnitOfWork unitOfWork,
            ITestExternalService testExternalService,
            IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.testExternalService = testExternalService;
            this.blobService = blobService;
        }

        public async Task<BasePaginationResponse<FeedbackWithReviewsResponse>> Handle(
            GetFeedbacksByTestIdQuery request, 
            CancellationToken cancellationToken)
        {
            var isTestExist = await testExternalService
                .TestIsExists(request.TestId, cancellationToken);

            if(!isTestExist)
            {
                throw new NotFoundException("Test doesnt exist");
            }

            await unitOfWork.BeginTransactionAsync(
                IsolationLevel.RepeatableRead, 
                cancellationToken);

            try
            {
                var feedbacks = await unitOfWork.FeedbackRepository
                    .GetFeedbacksByCriteria(
                    new FeedbacksByTestIdWithOwnerAndReviewsSpecification(request.TestId), 
                    request.Page,
                    request.PageSize,
                    cancellationToken);

                var allFeedbacks = await unitOfWork.FeedbackRepository
                    .GetFeedbacks(cancellationToken);

                var getFeedbacksImagesTasks = feedbacks
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
        
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return new BasePaginationResponse<FeedbackWithReviewsResponse>
                {
                    Items = feedbacksResponse,
                    Page = request.Page,
                    PageCount = request.PageSize,
                    MaxCount = allFeedbacks.Count()
                };
            }
            catch
            {
                await unitOfWork.RollBackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
