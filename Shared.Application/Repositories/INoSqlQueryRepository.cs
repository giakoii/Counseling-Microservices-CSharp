using System.Linq.Expressions;

namespace Shared.Application.Repositories;

public interface INoSqlQueryRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Find entities by predicate as asynchronous
    /// </summary>
    Task<List<TEntity>> FindAllAsync();

    /// <summary>
    /// Find entities by predicate
    /// </summary>
    /// <returns></returns>
    IEnumerable<TEntity> FindAll();
    
    /// <summary>
    /// Find entity by predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);
}