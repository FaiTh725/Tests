using System.Linq.Expressions;

namespace Notification.Domain.Primitives
{
    public abstract class BaseSpecification<TEntity> 
        where TEntity : class
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }
    
        public int? Page { get; protected set; }

        public int? PageSize { get; protected set; }

        public bool IsEnablePagination { get; protected set; }

        public Expression<Func<TEntity, object>>? OrderByExpression { get; protected set; }
        
        public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; protected set; }
    }
}
