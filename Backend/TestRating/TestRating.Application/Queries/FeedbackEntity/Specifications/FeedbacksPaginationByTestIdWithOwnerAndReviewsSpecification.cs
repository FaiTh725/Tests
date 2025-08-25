using TestRating.Domain.Entities;
using TestRating.Domain.Primitives;

namespace TestRating.Application.Queries.FeedbackEntity.Specifications
{
    public class FeedbacksPaginationByTestIdWithOwnerAndReviewsSpecification :
        Specification<Feedback>
    {
        public FeedbacksPaginationByTestIdWithOwnerAndReviewsSpecification(
            long testId, int page, int pageSize)
        {
            AddCriteria(feedback => feedback.TestId == testId);
            AddInclude(feedback => feedback.Owner);
            AddInclude(feedback => feedback.Reviews);
            AddOrderByDescending(x => x.UpdateTime);

            EnablePagination(page, pageSize);
        }
    }
}
