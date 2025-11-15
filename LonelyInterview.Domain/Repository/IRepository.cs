using LonelyInterview.Domain.Interfaces;


namespace LonelyInterview.Domain.Repository;

public interface IRepository<TEntity, TId> where TEntity : IEntity
{
    Task<TEntity?> GetByIdOrDefault(TId id, CancellationToken token = default);
    Task<IEnumerable<TEntity>> GetAll(CancellationToken token = default);
    Task Add(TEntity entity, CancellationToken token = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);

}
