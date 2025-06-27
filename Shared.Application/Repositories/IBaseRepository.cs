namespace Shared.Application.Repositories;

/// <summary>
/// Interface have common logic for command and query repositories.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IBaseRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Add entity to the database.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool Add(TEntity entity);
    
    /// <summary>
    /// Add entity to the database asynchronously.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task AddAsync(TEntity entity);
    
    /// <summary>
    /// Add a range of entities to the database asynchronously.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// Execute a function within a transaction.
    /// </summary>
    /// <param name="action"></param>
    void ExecuteInTransaction(Func<bool> action);
    
    /// <summary>
    /// Execute a function within a transaction asynchronously.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    Task ExecuteInTransactionAsync(Func<Task<bool>> action);
}