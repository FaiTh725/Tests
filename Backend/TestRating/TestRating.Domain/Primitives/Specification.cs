using System.Linq.Expressions;

namespace TestRating.Domain.Primitives
{
    public abstract class Specification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; private set; }

        public List<Expression<Func<T, object>>> IncludeExpressions { get; private set; } = new List<Expression<Func<T, object>>>();

        public List<string> IncludeExpressionsStrings { get; private set; } = new List<string>();

        public Expression<Func<T, object>>? OrderByExpression { get; private set; }

        public Expression<Func<T, object>>? OrderByDescendingExpression { get; private set; }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        }

        protected void AddInclude(string includeExpressionInStringFormat)
        {
            IncludeExpressionsStrings.Add(includeExpressionInStringFormat);
        }

        protected void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderByExpression = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescendingExpression = orderByDescendingExpression;
        }

    }
}
