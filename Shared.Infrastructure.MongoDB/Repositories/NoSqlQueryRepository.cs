using System.Linq.Expressions;
using MongoDB.Driver;
using Shared.Application.Repositories;

namespace Shared.Infrastructure.MongoDB.Repositories;

public class NoSqlQueryRepository<TEntity> : INoSqlQueryRepository<TEntity> where TEntity : class
{
    private readonly IMongoCollection<TEntity> _collection;
    private readonly IMongoDatabase _database;
    private readonly IClientSessionHandle? _session;

    public NoSqlQueryRepository(IMongoDatabase database, string collectionName = null)
    {
        _database = database;
        _collection = database.GetCollection<TEntity>(collectionName ?? typeof(TEntity).Name);
    }

    public NoSqlQueryRepository(IMongoDatabase database, IClientSessionHandle session, string collectionName = null) : this(database, collectionName)
    {
        _session = session;
    }

    public bool Add(TEntity entity)
    {
        if (_session != null)
        {
            _collection.InsertOne(_session, entity);
        }
        else
        {
            _collection.InsertOne(entity);
        }
        return true;
    }

    public async Task AddAsync(TEntity entity)
    {
        if (_session != null)
        {
            await _collection.InsertOneAsync(_session, entity);
        }
        else
        {
            await _collection.InsertOneAsync(entity);
        }
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        if (_session != null)
        {
            await _collection.InsertManyAsync(_session, entities);
        }
        else
        {
            await _collection.InsertManyAsync(entities);
        }
    }

    public void ExecuteInTransaction(Func<bool> action)
    {
        using var session = _database.Client.StartSession();
        session.StartTransaction();

        try
        {
            var result = action();
            if (result)
            {
                session.CommitTransaction();
            }
            else
            {
                session.AbortTransaction();
            }
        }
        catch
        {
            session.AbortTransaction();
            throw;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task<bool>> action)
    {
        using var session = await _database.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var result = await action();
            if (result)
            {
                await session.CommitTransactionAsync();
            }
            else
            {
                await session.AbortTransactionAsync();
            }
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
    
    /// <summary>
    /// Find entity by predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var filter = Builders<TEntity>.Filter.Where(predicate);

        if (_session != null)
        {
            return await _collection.Find(_session, filter).FirstOrDefaultAsync();
        }

        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Find entities by predicate
    /// </summary>
    /// <returns></returns>
    public async Task<List<TEntity>> FindAllAsync()
    {
        if (_session != null)
            return await _collection.Find(_session, Builders<TEntity>.Filter.Empty).ToListAsync();
        return await _collection.Find(Builders<TEntity>.Filter.Empty).ToListAsync();
    }
    
    /// <summary>
    /// Find entities by predicate
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TEntity> FindAll()
    {
        if (_session != null)
            return _collection.Find(_session, Builders<TEntity>.Filter.Empty).ToList();
        return _collection.Find(Builders<TEntity>.Filter.Empty).ToList();
    }
}