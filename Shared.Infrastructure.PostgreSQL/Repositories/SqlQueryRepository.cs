using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.PostgreSQL.Context;

namespace Shared.Infrastructure.PostgreSQL.Repositories;

public class SqlQueryRepository<TEntity>(AppDbContext context) : Application.Repositories.ISqlQueryRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context = context;

    private DbSet<TEntity> DbSet => Context.Set<TEntity>();
    

    public IQueryable<TViewEntity> GetView<TViewEntity>() where TViewEntity : class
    {
        return Context.Set<TViewEntity>().AsNoTracking();
    }

    public IQueryable<TViewEntity> GetView<TViewEntity>(Expression<Func<TViewEntity, bool>> predicate = null) where TViewEntity : class
    {
        return Context.Set<TViewEntity>().AsNoTracking().Where(predicate);
    }
}