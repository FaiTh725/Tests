namespace Test.Application.Common.Interfaces
{
    public interface ITempDbService<TEntity>
    {
        Task<TEntity> AddEntity(TEntity entity);

        Task<TEntity?> GetEntity(Guid id);

        Task RemoveEntity(Guid id);

        Task UpdateEntity(Guid entityId, TEntity updatedEntity);

        Task<IEnumerable<TEntity>> GetAllEntities();
    }
}
