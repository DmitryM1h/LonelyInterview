using LonelyInterview.Domain.Interfaces;
using LonelyInterview.Domain.Repository;

namespace LonelyInterview.Infrastructure.Data.DataSources;

using Microsoft.EntityFrameworkCore;

public abstract class AbstractRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IEntity
{
    protected readonly LonelyInterviewContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected AbstractRepository(LonelyInterviewContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdOrDefault(TId id, CancellationToken token = default)
    {
        return await _dbSet.FindAsync(id, token);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll(CancellationToken token = default)
    {
        return await _dbSet.ToListAsync(token);
    }

    public virtual async Task Add(TEntity entity, CancellationToken token = default)
    {
        await _dbSet.AddAsync(entity, token);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
      
    }

    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }


}
