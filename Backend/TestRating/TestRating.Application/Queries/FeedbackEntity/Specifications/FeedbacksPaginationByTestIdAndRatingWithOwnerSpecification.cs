using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackEntity.Specifications
{
    public class FeedbacksPaginationByTestIdAndRatingWithOwnerSpecification : 
        Specification<Feedback>
    {
        public FeedbacksPaginationByTestIdAndRatingWithOwnerSpecification(
            long testId, int rating, 
            int page, int pageSize)
        {
            AddCriteria(x =>
                x.TestId == testId &&
                x.Rating == rating);

            AddInclude(x => x.Owner);
            AddInclude(x => x.Reviews);

            AddOrderByDescending(x => x.UpdateTime);

            EnablePagination(page, pageSize);
        }
    }
}
