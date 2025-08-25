using MediatR;
using System.Data;
using TestRating.Application.Contacts.Test;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Queries.FeedbackEntity.GetTestStatisticsByFeedbacks
{
    public class GetTestStatisticsByFeedbacksHandler :
        IRequestHandler<GetTestStatisticsByFeedbacksQuery, TestFeedbackStatistics>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetTestStatisticsByFeedbacksHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<TestFeedbackStatistics> Handle(
            GetTestStatisticsByFeedbacksQuery request, 
            CancellationToken cancellationToken)
        {
            using var transaction = await unitOfWork
                .BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

            var averageRating = 0d;

            try 
            {
                averageRating = await unitOfWork.FeedbackRepository
                    .GetAverageRating(request.TestId, cancellationToken);
            }
            catch (InvalidOperationException)
            {
                averageRating = 0;
            }

            var ratingDistribution = await unitOfWork.FeedbackRepository
                .GetRatingDistribution(request.TestId, cancellationToken);

            await unitOfWork.CommitTransactionAsync(transaction, cancellationToken);

            return new TestFeedbackStatistics
            {
                TestId = request.TestId,
                RatingDistribution = ratingDistribution,
                AverageRating = Math.Round(averageRating, 1)
            };
        }
    }
}
