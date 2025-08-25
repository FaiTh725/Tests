using MongoDB.Driver;
using Notification.Domain.Primitives;
using Notification.Infrastructure.Data.Persistences;

namespace Notification.Infrastructure.Data.Specification
{
    public static class SpecificationEvaluator
    {
        public static IEnumerable<TEntity> GetQuery<TEntity, TMongoEntity>(
            IMongoCollection<TMongoEntity> collection,
            BaseSpecification<TEntity> specification) 
            where TEntity : class 
            where TMongoEntity : IMongoPersistence<TEntity, TMongoEntity>
        {
            var filter = specification.Criteria is null ?
                Builders<TMongoEntity>.Filter.Empty :
                new ExpressionConverter<TEntity, TMongoEntity>().Rewrite(specification.Criteria);
        
            var findResult = collection.Find(filter);

            if(specification.IsEnablePagination)
            {
                findResult
                    .Skip((specification.Page - 1) * specification.PageSize)
                    .Limit(specification.PageSize);
            }

            return findResult
                .Project(x => x.ConvertToDomainEntity())
                .ToList();
        }

        public static async Task<IEnumerable<TEntity>> GetQueryAsync<TEntity, TMongoEntity>(
            IMongoCollection<TMongoEntity> collection,
            BaseSpecification<TEntity> specification,
            CancellationToken cancellationToken = default)
            where TEntity : class
            where TMongoEntity : IMongoPersistence<TEntity, TMongoEntity>
        {
            var filter = specification.Criteria is null ?
                Builders<TMongoEntity>.Filter.Empty :
                new ExpressionConverter<TEntity, TMongoEntity>().Rewrite(specification.Criteria);

            var findResult = collection.Find(filter);

            if (specification.IsEnablePagination)
            {
                findResult
                    .Skip((specification.Page - 1) * specification.PageSize)
                    .Limit(specification.PageSize);
            }

            if(specification.OrderByExpression is not null)
            {
                var sort = Builders<TMongoEntity>.Sort.Ascending(
                    new ExpressionConverter<TEntity, TMongoEntity>()
                    .Rewrite(specification.OrderByExpression));

                findResult = findResult.Sort(sort);
            }

            if(specification.OrderByDescendingExpression is not null)
            {
                var sort = Builders<TMongoEntity>.Sort.Descending(
                    new ExpressionConverter<TEntity, TMongoEntity>()
                    .Rewrite(specification.OrderByDescendingExpression));

                findResult = findResult.Sort(sort);
            }

            var resultsList = await findResult.ToListAsync(cancellationToken);

            return resultsList.Select(x => x.ConvertToDomainEntity());
        }
    }
}
