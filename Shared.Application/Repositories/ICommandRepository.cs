using System.Linq.Expressions;

namespace Shared.Application.Repositories;

public interface ICommandRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Get records/ record based on a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    IQueryable<TEntity?> Find(Expression<Func<TEntity, bool>> predicate = null!, bool isTracking = true, params Expression<Func<TEntity, object>>[] includes);    

    /// <summary>
    /// Update entity in the database.
    /// </summary>
    /// <param name="entity"></param>
    void Update(TEntity entity);
    
    /// <summary>
    /// Save changes to the database with an optional logical delete flag.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="needLogicalDelete"></param>
    /// <returns></returns>
    int SaveChanges(string userName, bool needLogicalDelete = false);
    
    /// <summary>
    /// Save changes to the database asynchronously with an optional logical delete flag.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="needLogicalDelete"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(string userName, bool needLogicalDelete = false);
}