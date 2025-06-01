namespace Notification.Infrastructure.Data.Persistences
{
    public interface IMongoPersistence<TEntity, TMongoEntity>
    {
        public TMongoEntity ConvertToMongoEntity(TEntity entity);

        public TEntity ConvertToDomainEntity();
    }
}
