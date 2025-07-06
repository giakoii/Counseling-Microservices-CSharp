using System.Linq.Expressions;

namespace Shared.Application.Repositories;

public interface ISqlReadRepository<TEntity> where TEntity : class
{
    IQueryable<TViewEntity> GetView<TViewEntity>() where TViewEntity : class;

    IQueryable<TViewEntity> GetView<TViewEntity>(Expression<Func<TViewEntity, bool>> predicate = null) where TViewEntity : class;
}