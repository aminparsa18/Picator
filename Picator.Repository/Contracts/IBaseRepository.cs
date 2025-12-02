using Microsoft.EntityFrameworkCore.ChangeTracking;
using Picator.Data;
using Picator.Entities.Models;
using System.Linq.Expressions;

namespace Picator.Repository.Contracts;

/// <summary>
/// Base repository for common requests like insert, update, etc.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Inserts a new record in table of type TEntity.
    /// </summary>
    /// <param name="entity">new record.</param>
    ValueTask<EntityEntry<TEntity>> Add(TEntity entity);

    /// <summary>
    /// Inserts a list of new records in table of type TEntity.
    /// </summary>
    /// <param name="entities">list of new records.</param>
    Task AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an existing record in table of type TEntity.
    /// </summary>
    /// <param name="entity">existing record.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Updates list of existing records in table of type TEntity.
    /// </summary>
    /// <param name="entities">list of existing records.</param>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Remove an existing record from table of type TEntity.
    /// </summary>
    /// <param name="entity">existing record.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Remove list of existing records from table of type TEntity.
    /// </summary>
    /// <param name="entities">list of existing records.</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Count all records in table of type TEntity.
    /// </summary>
    Task<int> Count();

    /// <summary>
    /// Returns the record in table of type TEntity with defined primary key.
    /// </summary>
    /// <param name="id">Primary key of wanted record.</param>
    ValueTask<TEntity?> Find(string id);

    /// <summary>
    /// Returns the list of records in table of type TEntity with condition.
    /// </summary>
    /// <param name="predicate">Condition.</param>
    Task<List<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Returns the record in table of type TEntity with defined primary key.
    /// </summary>
    /// <param name="id">Primary key of wanted record.</param>
    Task<TEntity?> Get(Guid id);

    /// <summary>
    /// Returns the list of records in table of type TEntity with condition.
    /// </summary>
    /// <param name="predicate">Condition.</param>
    Task<TEntity?> Get(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Returns all records in table of type TEntity.
    /// </summary>
    Task<List<TEntity>> GetAll();

    /// <summary>
    /// Returns a flag indicating that if current entity exist table of type TEntity.
    /// </summary>
    Task<bool> Exists(TEntity entity);

    /// <summary>
    /// Returns records in table of type TEntity with pagination.
    /// </summary>
    /// <param name="args">Pagination arguments.</param>
    Task<List<TEntity>> GetPage(PaginationArgs args);

    /// <summary>
    /// Returns records in table of type TEntity with pagination.
    /// </summary>
    /// <param name="expression">Condition.</param>
    /// <param name="args">Pagination arguments.</param>
    Task<List<TEntity>> GetPage(Expression<Func<TEntity, bool>> expression, PaginationArgs args);
}