using System.Linq.Expressions;

namespace Test.Domain.Primitives
{
    public class BaseSpecification<TEntity>
        where TEntity : class
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; protected set; } 
    }
}
