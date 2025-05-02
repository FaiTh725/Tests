using Microsoft.EntityFrameworkCore;
using TestRating.Domain.Primitives;

namespace TestRating.Dal.Specification
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(
            IQueryable<T> inputQuery,
            Specification<T> specification)
            where T : class
        {
            IQueryable<T> query = inputQuery;

            if(specification.Criteria is not null)
            {
                query = query.Where(specification.Criteria);
            }

            if(specification.OrderByExpression is not null)
            {
                query = query.OrderBy(specification.OrderByExpression);
            }

            if(specification.OrderByDescendingExpression is not null)
            {
                query = query.OrderByDescending(specification.OrderByDescendingExpression);
            }

            query = specification.IncludeExpressions.Aggregate(
                query,
                (current, includeExpression) =>
                current.Include(includeExpression));

            query = specification.IncludeExpressionsStrings.Aggregate(
                query,
                (current, includeExpression) =>
                current.Include(includeExpression));

            return query;
        }
    }
}
