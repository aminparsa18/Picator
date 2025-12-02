using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> Entities;
    protected readonly IDbConnection Connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{TEntity, TDbConnection}"/> class.
    /// </summary>
    public BaseRepository(ApplicationDbContext context, IDbConnection connection)
    {
        Context = context;
        Connection = connection;
        Entities = context.Set<TEntity>();
    }

    /// <inheritdoc/>
    public virtual ValueTask<EntityEntry<TEntity>> Add(TEntity entity)
    {
        entity.Id = Guid.NewGuid();
        return Entities.AddAsync(entity);
    }

    /// <inheritdoc/>
    public virtual Task AddRange(IEnumerable<TEntity> entities)
    {
        foreach (var item in entities)
        {
            item.Id = Guid.NewGuid();
        }

        return Entities.AddRangeAsync(entities);
    }

    public virtual void Update(TEntity entity)
    {
        Entities.Update(entity);
    }

    /// <inheritdoc/>
    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        Entities.UpdateRange(entities);
    }

    /// <inheritdoc/>
    public virtual void Remove(TEntity entity)
    {
        Entities.Remove(entity);
    }

    /// <inheritdoc/>
    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        Entities.RemoveRange(entities);
    }

    /// <inheritdoc/>
    public virtual Task<int> Count()
    {
        return Entities.CountAsync();
    }

    /// <inheritdoc/>
    public virtual Task<TEntity?> Get(Expression<Func<TEntity, bool>> predicate)
    {
        return Entities.Where(predicate).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<List<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return Entities.Where(predicate).ToListAsync();
    }

    /// <inheritdoc/>
    public virtual Task<TEntity?> Get(Guid id)
    {
        return Entities.FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <inheritdoc/>
    public ValueTask<TEntity?> Find(string id)
    {
        return Entities.FindAsync(id);
    }

    /// <inheritdoc/>
    public virtual Task<List<TEntity>> GetAll()
    {
        return Entities.AsNoTracking().OrderByDescending(o => o.Id).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<List<TEntity>> GetPage(PaginationArgs args)
    {
        return Entities.AsNoTracking().Skip(args.StartingRow).Take(args.PageRowCount).OrderByDescending(o => o.Id).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<List<TEntity>> GetPage(Expression<Func<TEntity, bool>> expression, PaginationArgs args)
    {
        return Entities.AsNoTracking().Where(expression).Skip(args.StartingRow).Take(args.PageRowCount).OrderByDescending(o => o.Id)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public virtual Task<bool> Exists(TEntity entity) => Entities.ContainsAsync(entity);
}