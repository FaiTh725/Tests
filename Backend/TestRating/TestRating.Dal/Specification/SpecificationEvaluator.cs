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

            if(specification.IsEnablePagination)
            {
                if(specification.Page is null || specification.Page < 1 ||
                    specification.PageSize is null || specification.PageSize < 1)
                {
                    throw new InvalidOperationException("Unable to execute pagination with invalid page and page size");
                }

                var page = specification.Page.Value;
                var pageSize = specification.PageSize.Value;

                query = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);
            }

            query = specification.IncludeExpressions.Aggregate(
                query,
                (current, includeExpression) =>
                current.Include(includeExpression));

            return query;
        }
    }
}
